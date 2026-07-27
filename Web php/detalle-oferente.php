<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header('Location: index.php');
    exit;
}

require_once __DIR__ . '/data-fran.php';

$identificacion = trim($_GET['identificacion'] ?? $_POST['identificacion'] ?? '');
$idPuesto = (int) ($_GET['idPuesto'] ?? $_POST['idPuesto'] ?? 0);
$codigo = trim($_GET['codigo'] ?? $_POST['codigo'] ?? '');

$error = '';
$oferente = null;

// --- Acción: crear empleado (Core3) ---
if ($_SERVER['REQUEST_METHOD'] === 'POST' && ($_POST['accion'] ?? '') === 'crear_empleado') {
    $idOferente = (int) ($_POST['idOferente'] ?? 0);

    if ($idOferente <= 0 || $idPuesto <= 0) {
        $error = 'No se pudo determinar el oferente o el puesto seleccionado.';
    } else {
        $resultadoEmpleado = crearEmpleadoWCF($idOferente, $idPuesto);

        if ($resultadoEmpleado['exito']) {
            $_SESSION['mensajeExitoEmpleado'] = $resultadoEmpleado['mensaje'] !== ''
                ? $resultadoEmpleado['mensaje']
                : 'Empleado creado con éxito';
            header('Location: puestos.php');
            exit;
        }

        $error = $resultadoEmpleado['mensaje'] !== ''
            ? $resultadoEmpleado['mensaje']
            : 'No fue posible crear el empleado.';
    }
}

// --- Carga del detalle del oferente (Core8, de Andrew) ---
if ($identificacion === '') {
    $error = $error !== '' ? $error : 'No se indicó una identificación de oferente válida.';
} else {
    $resultadoDetalle = obtenerDetalleOferenteWCF($identificacion);

    if ($resultadoDetalle['exito']) {
        $oferente = $resultadoDetalle['oferente'];
    } elseif ($error === '') {
        $error = $resultadoDetalle['mensaje'] !== ''
            ? $resultadoDetalle['mensaje']
            : 'No se encontró la información del oferente solicitado.';
    }
}

function formatearFecha(string $fechaIso, bool $conHora = false): string
{
    if ($fechaIso === '') {
        return '—';
    }

    $timestamp = strtotime($fechaIso);
    if ($timestamp === false) {
        return htmlspecialchars($fechaIso);
    }

    return $conHora ? date('d/m/Y H:i', $timestamp) : date('d/m/Y', $timestamp);
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Detalle del Oferente - Administración de Personal</title>
    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background-color: #f2f4f7;
        }

        header {
            background-color: #1b3a63;
            color: #ffffff;
            padding: 16px 32px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        header .titulo {
            font-weight: 600;
        }

        header nav {
            display: flex;
            gap: 20px;
            align-items: center;
        }

        header a {
            color: #ffffff;
            text-decoration: none;
            font-size: 14px;
            padding: 8px 16px;
            border-radius: 4px;
            transition: background-color 0.3s ease;
        }

        header a:hover {
            background-color: rgba(255, 255, 255, 0.1);
        }

        header a.btn-inicio {
            background-color: #2563eb;
            font-weight: bold;
        }

        header a.btn-inicio:hover {
            background-color: #1e40af;
        }

        main {
            padding: 32px;
        }

        h1 {
            color: #1a1a1a;
        }

        .panel-acciones {
            background: #ffffff;
            border-left: 4px solid #2563eb;
            border-radius: 4px;
            padding: 16px 20px;
            margin-bottom: 18px;
            box-shadow: 0 3px 10px rgba(0, 0, 0, .06);
            max-width: 900px;
        }

        .boton-circular {
            width: 38px;
            height: 38px;
            border-radius: 50%;
            border: none;
            background-color: #5b7cfa;
            color: #ffffff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            text-decoration: none;
            font-size: 16px;
            cursor: pointer;
            margin-right: 8px;
        }

        .boton-circular:hover {
            background-color: #4169e1;
        }

        .tarjeta {
            background: #ffffff;
            border-radius: 4px;
            box-shadow: 0 3px 10px rgba(0, 0, 0, .08);
            padding: 24px;
            max-width: 900px;
            margin-bottom: 20px;
        }

        .tarjeta h2 {
            margin-top: 0;
            color: #243b73;
            font-size: 18px;
        }

        .fila-datos {
            display: flex;
            flex-wrap: wrap;
            gap: 20px 32px;
        }

        .dato {
            flex: 1 1 45%;
            min-width: 220px;
        }

        .dato label {
            display: block;
            color: #6c757d;
            font-size: 13px;
            margin-bottom: 2px;
        }

        .dato span {
            font-weight: bold;
            color: #1a1a1a;
        }

        table.subtabla {
            width: 100%;
            border-collapse: collapse;
            margin-top: 8px;
        }

        table.subtabla th, table.subtabla td {
            padding: 8px 10px;
            text-align: left;
            border-bottom: 1px solid #e5e7eb;
            font-size: 14px;
        }

        table.subtabla thead th {
            background-color: #f2f4f7;
            color: #495057;
        }

        .sin-datos {
            color: #6c757d;
            font-style: italic;
            font-size: 14px;
        }

        .aviso-error {
            background-color: #f8d7da;
            color: #842029;
            border: 1px solid #f5c2c7;
            border-radius: 4px;
            padding: 12px 16px;
            margin-bottom: 16px;
            max-width: 900px;
        }
    </style>
</head>
<body>
    <header>
        <span class="titulo">Administración de Personal - Servicios Médicos SA</span>
        <nav>
            <a href="bienvenida.php" class="btn-inicio">Inicio</a>
            <a href="logout.php">Cerrar sesión</a>
        </nav>
    </header>

    <main>
        <h1>Detalle del Oferente</h1>

        <?php if ($error !== ''): ?>
            <div class="aviso-error"><?= htmlspecialchars($error) ?></div>
        <?php endif; ?>

        <?php if ($oferente !== null): ?>
            <form method="post" action="detalle-oferente.php">
                <input type="hidden" name="accion" value="crear_empleado">
                <input type="hidden" name="identificacion" value="<?= htmlspecialchars($identificacion) ?>">
                <input type="hidden" name="idPuesto" value="<?= (int) $idPuesto ?>">
                <input type="hidden" name="codigo" value="<?= htmlspecialchars($codigo) ?>">
                <input type="hidden" name="idOferente" value="<?= (int) $oferente['idOferente'] ?>">

                <div class="panel-acciones">
                    <a class="boton-circular" href="oferentes.php?codigo=<?= urlencode($codigo) ?>&idPuesto=<?= (int) $idPuesto ?>" title="Cancelar">&#8592;</a>
                    <button type="submit" class="boton-circular" title="Crear empleado">&#10003;</button>
                </div>

                <div class="tarjeta">
                    <h2>Información personal</h2>
                    <div class="fila-datos">
                        <div class="dato">
                            <label>Identificación</label>
                            <span><?= htmlspecialchars($oferente['identificacion']) ?> (<?= htmlspecialchars($oferente['tipoIdentificacion']) ?>)</span>
                        </div>
                        <div class="dato">
                            <label>Nombre completo</label>
                            <span><?= htmlspecialchars($oferente['nombreCompleto']) ?></span>
                        </div>
                        <div class="dato">
                            <label>Fecha de nacimiento</label>
                            <span><?= formatearFecha($oferente['fechaNacimiento']) ?></span>
                        </div>
                        <div class="dato">
                            <label>Dirección</label>
                            <span><?= $oferente['direccion'] !== '' ? htmlspecialchars($oferente['direccion']) : '—' ?></span>
                        </div>
                        <?php if ($oferente['nombreDistrito'] !== ''): ?>
                            <div class="dato">
                                <label>Ubicación</label>
                                <span>
                                    <?= htmlspecialchars($oferente['nombreDistrito']) ?>,
                                    <?= htmlspecialchars($oferente['nombreCanton']) ?>,
                                    <?= htmlspecialchars($oferente['nombreProvincia']) ?>
                                </span>
                            </div>
                        <?php endif; ?>
                        <div class="dato">
                            <label>Correo(s)</label>
                            <span><?= !empty($oferente['correos']) ? htmlspecialchars(implode(', ', $oferente['correos'])) : '—' ?></span>
                        </div>
                        <div class="dato">
                            <label>Teléfono(s)</label>
                            <span><?= !empty($oferente['telefonos']) ? htmlspecialchars(implode(', ', $oferente['telefonos'])) : '—' ?></span>
                        </div>
                        <div class="dato">
                            <label>Fecha de registro</label>
                            <span><?= formatearFecha($oferente['fechaRegistro'], true) ?></span>
                        </div>
                    </div>
                </div>

                <div class="tarjeta">
                    <h2>Preparación académica</h2>
                    <?php if (empty($oferente['preparacionAcademica'])): ?>
                        <p class="sin-datos">No hay preparación académica registrada.</p>
                    <?php else: ?>
                        <table class="subtabla">
                            <thead>
                                <tr><th>Institución</th><th>Título</th><th>Desde</th><th>Hasta</th></tr>
                            </thead>
                            <tbody>
                                <?php foreach ($oferente['preparacionAcademica'] as $prep): ?>
                                    <tr>
                                        <td><?= htmlspecialchars($prep['institucion']) ?></td>
                                        <td><?= htmlspecialchars($prep['titulo']) ?></td>
                                        <td><?= formatearFecha($prep['fechaInicio']) ?></td>
                                        <td><?= formatearFecha($prep['fechaFin']) ?></td>
                                    </tr>
                                <?php endforeach; ?>
                            </tbody>
                        </table>
                    <?php endif; ?>
                </div>

                <div class="tarjeta">
                    <h2>Experiencia laboral</h2>
                    <?php if (empty($oferente['experienciaLaboral'])): ?>
                        <p class="sin-datos">No hay experiencia laboral registrada.</p>
                    <?php else: ?>
                        <table class="subtabla">
                            <thead>
                                <tr><th>Empresa</th><th>Puesto</th><th>Desde</th><th>Hasta</th></tr>
                            </thead>
                            <tbody>
                                <?php foreach ($oferente['experienciaLaboral'] as $exp): ?>
                                    <tr>
                                        <td><?= htmlspecialchars($exp['empresa']) ?></td>
                                        <td><?= htmlspecialchars($exp['puesto']) ?></td>
                                        <td><?= formatearFecha($exp['fechaInicio']) ?></td>
                                        <td><?= formatearFecha($exp['fechaFin']) ?></td>
                                    </tr>
                                <?php endforeach; ?>
                            </tbody>
                        </table>
                    <?php endif; ?>
                </div>

                <div class="tarjeta">
                    <h2>Postulaciones</h2>
                    <?php if (empty($oferente['postulaciones'])): ?>
                        <p class="sin-datos">No hay postulaciones registradas.</p>
                    <?php else: ?>
                        <table class="subtabla">
                            <thead>
                                <tr><th>Puesto</th><th>Fecha</th><th>Estado</th></tr>
                            </thead>
                            <tbody>
                                <?php foreach ($oferente['postulaciones'] as $post): ?>
                                    <tr>
                                        <td><?= htmlspecialchars($post['codigoPuesto']) ?> - <?= htmlspecialchars($post['nombrePuesto']) ?></td>
                                        <td><?= formatearFecha($post['fechaPostulacion']) ?></td>
                                        <td><?= htmlspecialchars($post['estado']) ?></td>
                                    </tr>
                                <?php endforeach; ?>
                            </tbody>
                        </table>
                    <?php endif; ?>
                </div>
            </form>
        <?php endif; ?>
    </main>
</body>
</html>
