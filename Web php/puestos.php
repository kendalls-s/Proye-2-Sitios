<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header('Location: index.php');
    exit;
}

require_once __DIR__ . '/data-fran.php';

$mensajeExito = $_SESSION['mensajeExitoEmpleado'] ?? '';
unset($_SESSION['mensajeExitoEmpleado']);

$resultado = obtenerPuestosActivosWCF();
$puestos = $resultado['puestos'];
$error = $resultado['exito'] ? '' : $resultado['mensaje'];
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Puestos Activos - Administración de Personal</title>
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

        header a {
            color: #ffffff;
            text-decoration: none;
            font-size: 14px;
        }

        main {
            padding: 32px;
        }

        h1 {
            color: #1a1a1a;
        }

        .tarjeta {
            background: #ffffff;
            border-radius: 4px;
            box-shadow: 0 3px 10px rgba(0, 0, 0, .08);
            padding: 24px;
            max-width: 800px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 16px;
        }

        th, td {
            padding: 10px 12px;
            text-align: left;
            border-bottom: 1px solid #e5e7eb;
        }

        thead th {
            background-color: #343a40;
            color: #ffffff;
        }

        tbody tr:hover {
            background-color: #f8f9fa;
        }

        a.enlace-puesto {
            color: #2563eb;
            text-decoration: none;
            font-weight: bold;
        }

        a.enlace-puesto:hover {
            text-decoration: underline;
        }

        .aviso-exito {
            background-color: #d1e7dd;
            color: #0f5132;
            border: 1px solid #badbcc;
            border-radius: 4px;
            padding: 12px 16px;
            margin-bottom: 16px;
        }

        .aviso-error {
            background-color: #f8d7da;
            color: #842029;
            border: 1px solid #f5c2c7;
            border-radius: 4px;
            padding: 12px 16px;
            margin-bottom: 16px;
        }

        .sin-datos {
            color: #6c757d;
            font-style: italic;
        }
    </style>
</head>
<body>
    <header>
        <span>Administración de Personal - Servicios Médicos SA</span>
        <a href="logout.php">Cerrar sesión</a>
    </header>

    <main>
        <h1>Puestos Activos</h1>

        <div class="tarjeta">
            <?php if ($mensajeExito !== ''): ?>
                <div class="aviso-exito"><?= htmlspecialchars($mensajeExito) ?></div>
            <?php endif; ?>

            <?php if ($error !== ''): ?>
                <div class="aviso-error"><?= htmlspecialchars($error) ?></div>
            <?php endif; ?>

            <p>Seleccione un puesto para ver los oferentes que cumplen sus requisitos.</p>

            <table>
                <thead>
                    <tr>
                        <th>Código</th>
                        <th>Puesto</th>
                    </tr>
                </thead>
                <tbody>
                    <?php if (empty($puestos)): ?>
                        <tr>
                            <td colspan="2" class="sin-datos">No hay puestos activos en este momento.</td>
                        </tr>
                    <?php else: ?>
                        <?php foreach ($puestos as $puesto): ?>
                            <tr>
                                <td><?= htmlspecialchars($puesto['codigo']) ?></td>
                                <td>
                                    <a class="enlace-puesto"
                                       href="oferentes.php?codigo=<?= urlencode($puesto['codigo']) ?>&idPuesto=<?= (int) $puesto['idPuesto'] ?>">
                                        <?= htmlspecialchars($puesto['nombre']) ?>
                                    </a>
                                </td>
                            </tr>
                        <?php endforeach; ?>
                    <?php endif; ?>
                </tbody>
            </table>
        </div>
    </main>
</body>
</html>
