<?php

require_once __DIR__ . '/config.php';

function autenticarWCF(string $usuario, string $password): array
{
    $xml = <<<XML
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:tem="http://tempuri.org/"
                  xmlns:mod="{SOAP_NAMESPACE_MODELOS}">
   <soapenv:Header/>
   <soapenv:Body>
      <tem:Autenticar>
         <tem:credenciales>
            <mod:Usuario>{$usuario}</mod:Usuario>
            <mod:Password>{$password}</mod:Password>
         </tem:credenciales>
      </tem:Autenticar>
   </soapenv:Body>
</soapenv:Envelope>
XML;

    $xml = str_replace('{SOAP_NAMESPACE_MODELOS}', SOAP_NAMESPACE_MODELOS, $xml);

    $ch = curl_init(WCF_URL);
    curl_setopt_array($ch, [
        CURLOPT_POST => true,
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_HTTPHEADER => [
            'Content-Type: text/xml; charset=utf-8',
            'SOAPAction: "' . SOAP_ACTION . '"',
        ],
        CURLOPT_POSTFIELDS => $xml,
    ]);
    $respuesta = curl_exec($ch);
    curl_close($ch);

    if ($respuesta === false) {
        return ['autenticado' => false, 'mensaje' => 'No se pudo conectar con el servicio de autenticación.', 'nombreCompleto' => ''];
    }

    $doc = new DOMDocument();
    libxml_use_internal_errors(true);
    $doc->loadXML($respuesta);
    libxml_clear_errors();

    $autenticado = $doc->getElementsByTagName('Autenticado')->item(0);
    $mensaje = $doc->getElementsByTagName('Mensaje')->item(0);
    $nombreCompleto = $doc->getElementsByTagName('NombreCompleto')->item(0);

    return [
        'autenticado' => $autenticado && strtolower(trim($autenticado->textContent)) === 'true',
        'mensaje' => $mensaje ? trim($mensaje->textContent) : '',
        'nombreCompleto' => $nombreCompleto ? trim($nombreCompleto->textContent) : '',
    ];
}
