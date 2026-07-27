<?php

// Clientes SOAP (cURL + XML crudo + DOMDocument) para los Web Services de
// Fran: Core1 (Puestos), Core3 (Empleados) y el consumo de Core8 (Andrew,
// dentro de ServicioOferentes.svc) que necesita Core9.
//
// Sigue exactamente el mismo patrón que data.php (Login, de Kendall):
// no se usa SoapClient de PHP, se arma el sobre SOAP a mano y se
// interpreta la respuesta con DOMDocument.
//
// IMPORTANTE: a diferencia de autenticarWCF() (que siempre recibe un
// ResultadoAutenticacion con Mensaje), Core1 y Core8 devuelven listas u
// objetos "planos": un error técnico del lado del servidor no viaja como
// un campo Mensaje, sino como un SOAP Fault real. Por eso cada función
// aquí primero revisa si la respuesta es un <s:Fault> antes de intentar
// leer los datos.

require_once __DIR__ . '/config-fran.php';

/**
 * Ejecuta una petición SOAP 1.1 vía cURL contra alguno de los servicios WCF.
 *
 * @return array{ok: bool, body: string, error: string}
 */
function ejecutarPeticionSoap(string $url, string $soapAction, string $xmlSobre): array
{
    $ch = curl_init($url);

    curl_setopt_array($ch, [
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_POST => true,
        CURLOPT_POSTFIELDS => $xmlSobre,
        CURLOPT_HTTPHEADER => [
            'Content-Type: text/xml; charset=utf-8',
            'SOAPAction: "' . $soapAction . '"',
        ],
        CURLOPT_TIMEOUT => 15,
    ]);

    $respuesta = curl_exec($ch);
    $error = curl_error($ch);
    curl_close($ch);

    if ($respuesta === false) {
        return ['ok' => false, 'body' => '', 'error' => $error !== '' ? $error : 'No fue posible contactar al servicio.'];
    }

    return ['ok' => true, 'body' => $respuesta, 'error' => ''];
}

function escaparParaXml(string $valor): string
{
    return htmlspecialchars($valor, ENT_QUOTES | ENT_XML1, 'UTF-8');
}

/**
 * Busca, dentro de un nodo, el primer elemento con el nombre de etiqueta dado
 * y devuelve su texto (o $porDefecto si no existe).
 */
function textoDeEtiqueta(DOMNode $nodo, string $etiqueta, string $porDefecto = ''): string
{
    if (!($nodo instanceof DOMElement) && !($nodo instanceof DOMDocument)) {
        return $porDefecto;
    }

    $elementos = $nodo->getElementsByTagName($etiqueta);

    return $elementos->length > 0 ? trim($elementos->item(0)->textContent) : $porDefecto;
}

/**
 * Devuelve el mensaje de un SOAP Fault si la respuesta es un fault, o ''
 * si la respuesta es una respuesta normal (no-fault).
 */
function mensajeDeFaultSoap(DOMDocument $doc): string
{
    $fault = $doc->getElementsByTagName('Fault')->item(0);
    if ($fault === null) {
        return '';
    }

    $faultString = $fault->getElementsByTagName('faultstring')->item(0)
        ?? $fault->getElementsByTagName('Reason')->item(0);

    return $faultString !== null ? trim($faultString->textContent) : 'El servicio reportó un error.';
}

// =====================================================================
// Core1 - Puestos activos
// =====================================================================

/**
 * Consume Core1 (ObtenerPuestosActivos) y devuelve la lista de puestos activos.
 *
 * @return array{exito: bool, mensaje: string, puestos: array<int, array{idPuesto:int, codigo:string, nombre:string}>}
 */
function obtenerPuestosActivosWCF(): array
{
    $xmlSobre = <<<XML
<?xml version="1.0" encoding="utf-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
   <soapenv:Header/>
   <soapenv:Body>
      <tem:ObtenerPuestosActivos/>
   </soapenv:Body>
</soapenv:Envelope>
XML;

    $peticion = ejecutarPeticionSoap(WCF_URL_PUESTOS, SOAP_ACTION_PUESTOS, $xmlSobre);

    if (!$peticion['ok']) {
        return ['exito' => false, 'mensaje' => $peticion['error'], 'puestos' => []];
    }

    $doc = new DOMDocument();
    if (!@$doc->loadXML($peticion['body'])) {
        return ['exito' => false, 'mensaje' => 'Respuesta inválida del servicio de puestos.', 'puestos' => []];
    }

    $mensajeFault = mensajeDeFaultSoap($doc);
    if ($mensajeFault !== '') {
        return ['exito' => false, 'mensaje' => $mensajeFault, 'puestos' => []];
    }

    $puestos = [];
    foreach ($doc->getElementsByTagName('PuestoActivo') as $item) {
        $puestos[] = [
            'idPuesto' => (int) textoDeEtiqueta($item, 'IdPuesto', '0'),
            'codigo' => textoDeEtiqueta($item, 'Codigo'),
            'nombre' => textoDeEtiqueta($item, 'Nombre'),
        ];
    }

    return ['exito' => true, 'mensaje' => '', 'puestos' => $puestos];
}

// =====================================================================
// Core3 - Creación de empleados
// =====================================================================

/**
 * Consume Core3 (CrearEmpleado).
 *
 * @return array{exito: bool, mensaje: string, idEmpleado: int, numeroEmpleado: string}
 */
function crearEmpleadoWCF(int $idOferente, int $idPuesto, ?string $fechaIngreso = null): array
{
    $fecha = $fechaIngreso ?? date('Y-m-d\TH:i:s');
    $namespaceModelos = SOAP_NAMESPACE_MODELOS;

    $xmlSobre = <<<XML
<?xml version="1.0" encoding="utf-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                   xmlns:tem="http://tempuri.org/"
                   xmlns:mod="{$namespaceModelos}">
   <soapenv:Header/>
   <soapenv:Body>
      <tem:CrearEmpleado>
         <tem:solicitud>
            <mod:IdOferente>{$idOferente}</mod:IdOferente>
            <mod:IdPuesto>{$idPuesto}</mod:IdPuesto>
            <mod:FechaIngreso>{$fecha}</mod:FechaIngreso>
         </tem:solicitud>
      </tem:CrearEmpleado>
   </soapenv:Body>
</soapenv:Envelope>
XML;

    $peticion = ejecutarPeticionSoap(WCF_URL_EMPLEADOS, SOAP_ACTION_EMPLEADOS, $xmlSobre);

    if (!$peticion['ok']) {
        return ['exito' => false, 'mensaje' => $peticion['error'], 'idEmpleado' => 0, 'numeroEmpleado' => ''];
    }

    $doc = new DOMDocument();
    if (!@$doc->loadXML($peticion['body'])) {
        return ['exito' => false, 'mensaje' => 'Respuesta inválida del servicio de empleados.', 'idEmpleado' => 0, 'numeroEmpleado' => ''];
    }

    $mensajeFault = mensajeDeFaultSoap($doc);
    if ($mensajeFault !== '') {
        return ['exito' => false, 'mensaje' => $mensajeFault, 'idEmpleado' => 0, 'numeroEmpleado' => ''];
    }

    $creadoNodo = $doc->getElementsByTagName('Creado')->item(0);
    $creado = $creadoNodo !== null && strtolower(trim($creadoNodo->textContent)) === 'true';

    return [
        'exito' => $creado,
        'mensaje' => textoDeEtiqueta($doc, 'Mensaje'),
        'idEmpleado' => (int) textoDeEtiqueta($doc, 'IdEmpleado', '0'),
        'numeroEmpleado' => textoDeEtiqueta($doc, 'NumeroEmpleado'),
    ];
}

// =====================================================================
// Core8 (Andrew, dentro de ServicioOferentes.svc) - Detalle de oferente
// =====================================================================

/**
 * Consume ObtenerDetalleOferente (Core8) por identificación.
 * El servicio devuelve el objeto directamente (null si no existe), no un
 * envoltorio Exito/Mensaje, así que "no encontrado" se detecta viendo si
 * el resultado viene vacío/xsi:nil.
 *
 * @return array{exito: bool, mensaje: string, oferente: array<string, mixed>|null}
 */
function obtenerDetalleOferenteWCF(string $identificacion): array
{
    $identificacionSegura = escaparParaXml($identificacion);

    $xmlSobre = <<<XML
<?xml version="1.0" encoding="utf-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
   <soapenv:Header/>
   <soapenv:Body>
      <tem:ObtenerDetalleOferente>
         <tem:identificacion>{$identificacionSegura}</tem:identificacion>
      </tem:ObtenerDetalleOferente>
   </soapenv:Body>
</soapenv:Envelope>
XML;

    $peticion = ejecutarPeticionSoap(WCF_URL_OFERENTES, SOAP_ACTION_DETALLE_OFERENTE, $xmlSobre);

    if (!$peticion['ok']) {
        return ['exito' => false, 'mensaje' => $peticion['error'], 'oferente' => null];
    }

    $doc = new DOMDocument();
    if (!@$doc->loadXML($peticion['body'])) {
        return ['exito' => false, 'mensaje' => 'Respuesta inválida del servicio de oferentes.', 'oferente' => null];
    }

    $mensajeFault = mensajeDeFaultSoap($doc);
    if ($mensajeFault !== '') {
        return ['exito' => false, 'mensaje' => $mensajeFault, 'oferente' => null];
    }

    $resultadoNodo = $doc->getElementsByTagName('ObtenerDetalleOferenteResult')->item(0);

    // El servidor devuelve null cuando no existe el oferente: WCF lo serializa
    // como un elemento con xsi:nil="true" (o vacío, sin hijos con datos).
    if ($resultadoNodo === null || $resultadoNodo->getElementsByTagName('IdOferente')->length === 0) {
        return ['exito' => false, 'mensaje' => 'No se encontró la información del oferente solicitado.', 'oferente' => null];
    }

    $oferente = [
        'idOferente' => (int) textoDeEtiqueta($resultadoNodo, 'IdOferente', '0'),
        'identificacion' => textoDeEtiqueta($resultadoNodo, 'Identificacion'),
        'tipoIdentificacion' => textoDeEtiqueta($resultadoNodo, 'TipoIdentificacion'),
        'nombreCompleto' => textoDeEtiqueta($resultadoNodo, 'NombreCompleto'),
        'fechaNacimiento' => textoDeEtiqueta($resultadoNodo, 'FechaNacimiento'),
        'direccion' => textoDeEtiqueta($resultadoNodo, 'Direccion'),
        'nombreDistrito' => textoDeEtiqueta($resultadoNodo, 'NombreDistrito'),
        'nombreCanton' => textoDeEtiqueta($resultadoNodo, 'NombreCanton'),
        'nombreProvincia' => textoDeEtiqueta($resultadoNodo, 'NombreProvincia'),
        'fechaRegistro' => textoDeEtiqueta($resultadoNodo, 'FechaRegistro'),
        'correos' => [],
        'telefonos' => [],
        'preparacionAcademica' => [],
        'experienciaLaboral' => [],
        'curriculums' => [],
        'postulaciones' => [],
    ];

    $correosNodo = $resultadoNodo->getElementsByTagName('Correos')->item(0);
    if ($correosNodo !== null) {
        foreach ($correosNodo->getElementsByTagName('string') as $c) {
            $oferente['correos'][] = trim($c->textContent);
        }
    }

    $telefonosNodo = $resultadoNodo->getElementsByTagName('Telefonos')->item(0);
    if ($telefonosNodo !== null) {
        foreach ($telefonosNodo->getElementsByTagName('string') as $t) {
            $oferente['telefonos'][] = trim($t->textContent);
        }
    }

    foreach ($resultadoNodo->getElementsByTagName('PreparacionAcademicaOferente') as $p) {
        $oferente['preparacionAcademica'][] = [
            'institucion' => textoDeEtiqueta($p, 'Institucion'),
            'titulo' => textoDeEtiqueta($p, 'Titulo'),
            'fechaInicio' => textoDeEtiqueta($p, 'FechaInicio'),
            'fechaFin' => textoDeEtiqueta($p, 'FechaFin'),
        ];
    }

    foreach ($resultadoNodo->getElementsByTagName('ExperienciaLaboralOferente') as $e) {
        $oferente['experienciaLaboral'][] = [
            'empresa' => textoDeEtiqueta($e, 'Empresa'),
            'puesto' => textoDeEtiqueta($e, 'Puesto'),
            'fechaInicio' => textoDeEtiqueta($e, 'FechaInicio'),
            'fechaFin' => textoDeEtiqueta($e, 'FechaFin'),
        ];
    }

    foreach ($resultadoNodo->getElementsByTagName('CurriculumOferente') as $c) {
        $oferente['curriculums'][] = [
            'nombreArchivo' => textoDeEtiqueta($c, 'NombreArchivo'),
            'rutaArchivo' => textoDeEtiqueta($c, 'RutaArchivo'),
            'fechaCarga' => textoDeEtiqueta($c, 'FechaCarga'),
        ];
    }

    foreach ($resultadoNodo->getElementsByTagName('PostulacionOferente') as $p) {
        $oferente['postulaciones'][] = [
            'codigoPuesto' => textoDeEtiqueta($p, 'CodigoPuesto'),
            'nombrePuesto' => textoDeEtiqueta($p, 'NombrePuesto'),
            'fechaPostulacion' => textoDeEtiqueta($p, 'FechaPostulacion'),
            'estado' => textoDeEtiqueta($p, 'Estado'),
            'observacion' => textoDeEtiqueta($p, 'Observacion'),
        ];
    }

    return ['exito' => true, 'mensaje' => '', 'oferente' => $oferente];
}

// =====================================================================
// Core2 - Oferentes aptos para un puesto
// =====================================================================

/**
 * Consume ObtenerOferentesAptos (Core2) y devuelve lista de oferentes
 * que cumplen los requisitos del puesto.
 *
 * @return array{exito: bool, mensaje: string, oferentes: array<int, array{idOferente:int, nombre:string, identificacion:string}>}
 */
function obtenerOfertesAptosWCF(string $codigoPuesto): array
{
    $codigoSeguro = escaparParaXml($codigoPuesto);

    $xmlSobre = <<<XML
<?xml version="1.0" encoding="utf-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
   <soapenv:Header/>
   <soapenv:Body>
      <tem:ObtenerOferentesAptos>
         <tem:codigoPuesto>{$codigoSeguro}</tem:codigoPuesto>
      </tem:ObtenerOferentesAptos>
   </soapenv:Body>
</soapenv:Envelope>
XML;

    $peticion = ejecutarPeticionSoap(WCF_URL_OFERENTES, SOAP_ACTION_OFERENTES_APTOS, $xmlSobre);

    if (!$peticion['ok']) {
        return ['exito' => false, 'mensaje' => $peticion['error'], 'oferentes' => []];
    }

    $doc = new DOMDocument();
    if (!@$doc->loadXML($peticion['body'])) {
        return ['exito' => false, 'mensaje' => 'Respuesta inválida del servicio de oferentes.', 'oferentes' => []];
    }

    $mensajeFault = mensajeDeFaultSoap($doc);
    if ($mensajeFault !== '') {
        return ['exito' => false, 'mensaje' => $mensajeFault, 'oferentes' => []];
    }

    $oferentes = [];
    foreach ($doc->getElementsByTagName('OferenteApto') as $item) {
        $oferentes[] = [
            'idOferente' => (int) textoDeEtiqueta($item, 'IdOferente', '0'),
            'nombre' => textoDeEtiqueta($item, 'NombreCompleto'),
            'identificacion' => textoDeEtiqueta($item, 'Identificacion'),
        ];
    }

    return ['exito' => true, 'mensaje' => '', 'oferentes' => $oferentes];
}
