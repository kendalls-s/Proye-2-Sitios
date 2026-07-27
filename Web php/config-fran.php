<?php

// Configuración de los Web Services de Fran (Core1, Core3) y del consumo de
// Core8 (Andrew) que necesita Core9. Todo vive en el MISMO sitio WCF que
// ServicioAutenticacion/ServicioOferentes (mismo puerto), solo cambia el
// nombre del .svc. Se mantiene en un archivo separado de config.php (Kendall)
// para no generar conflictos al fusionar ramas.

define('WCF_BASE_URL', 'http://localhost:62310');

define('WCF_URL_PUESTOS', WCF_BASE_URL . '/ServicioPuestos.svc');
define('SOAP_ACTION_PUESTOS', 'http://tempuri.org/IServicioPuestos/ObtenerPuestosActivos');

define('WCF_URL_EMPLEADOS', WCF_BASE_URL . '/ServicioEmpleados.svc');
define('SOAP_ACTION_EMPLEADOS', 'http://tempuri.org/IServicioEmpleados/CrearEmpleado');

// Core8 (Andrew) - vive en ServicioOferentes.svc junto con Core2.
define('WCF_URL_OFERENTES', WCF_BASE_URL . '/ServicioOferentes.svc');
define('SOAP_ACTION_DETALLE_OFERENTE', 'http://tempuri.org/IServicioOferentes/ObtenerDetalleOferente');

define('SOAP_NAMESPACE_MODELOS', 'http://schemas.datacontract.org/2004/07/AdministracionPersonal.WebService.Modelos');
