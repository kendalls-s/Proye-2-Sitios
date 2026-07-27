<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header('Location: index.php');
    exit;
}

require_once __DIR__ . '/data-fran.php';

$codigo = trim($_GET['codigo'] ?? '');
$idPuesto = (int) ($_GET['idPuesto'] ?? 0);

$error = '';
$oferentes = [];

if ($codigo === '' || $idPuesto <= 0) {
    $error = 'No se indicó un puesto válido.';
} else {
    $resultado = obtenerOfertesAptosWCF($codigo);

    if ($resultado['exito']) {
        $oferentes = $resultado['oferentes'];
    } else {
        $error = $resultado['mensaje'] !== ''
            ? $resultado['mensaje']
            : 'No fue posible obtener la lista de oferentes.';
    }
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Oferentes - Administración de Personal</title>
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

        header a.btn-puestos {
            background-color: #2563eb;
            font-weight: bold;
        }

        header a.btn-puestos:hover {
            background-color: #1e40af;
        }

        main {
            padding: 32px;
        }

        h1 {
            color: #1a1a1a;
        }

        .breadcrumb {
            margin-bottom: 24px;
            font-size: 14px;
        }

        .breadcrumb a {
            color: #2563eb;
            text-decoration: none;
        }

        .breadcrumb a:hover {
            text-decoration: underline;
        }

        .tarjeta {
            background: #ffffff;
            border-radius: 4px;
            box-shadow: 0 3px 10px rgba(0, 0, 0, .08);
            padding: 24px;
            max-width: 900px;
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

        a.enlace-oferente {
            color: #2563eb;
            text-decoration: none;
            font-weight: bold;
        }

        a.enlace-oferente:hover {
            text-decoration: underline;
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

        .info-puesto {
            background-color: #e3f2fd;
            border-left: 4px solid #2196f3;
            padding: 12px 16px;
            margin-bottom: 16px;
            border-radius: 4px;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <header>
        <span class="titulo">Administración de Personal - Servicios Médicos SA</span>
        <nav>
            <a href="bienvenida.php" class="btn-puestos">Inicio</a>
            <a href="logout.php">Cerrar sesión</a>
        </nav>
    </header>

    <main>

        <h1>Oferentes Disponibles</h1>

        <div class="tarjeta">
            <?php if ($codigo !== '' && $idPuesto > 0): ?>
                <div class="info-puesto">
                    <strong>Puesto:</strong> <?= htmlspecialchars($codigo) ?>
                    (ID: <?= (int) $idPuesto ?>)
                </div>
            <?php endif; ?>

            <?php if ($error !== ''): ?>
                <div class="aviso-error"><?= htmlspecialchars($error) ?></div>
            <?php endif; ?>

            <p>Seleccione un oferente para ver su detalle y crear un empleado.</p>

            <table>
                <thead>
                    <tr>
                        <th>Identificación</th>
                        <th>Nombre</th>
                    </tr>
                </thead>
                <tbody>
                    <?php if (empty($oferentes)): ?>
                        <tr>
                            <td colspan="2" class="sin-datos">No hay oferentes disponibles para este puesto.</td>
                        </tr>
                    <?php else: ?>
                        <?php foreach ($oferentes as $oferente): ?>
                            <tr>
                                <td><?= htmlspecialchars($oferente['identificacion']) ?></td>
                                <td>
                                    <a class="enlace-oferente"
                                       href="detalle-oferente.php?identificacion=<?= urlencode($oferente['identificacion']) ?>&idPuesto=<?= (int) $idPuesto ?>&codigo=<?= urlencode($codigo) ?>">
                                        <?= htmlspecialchars($oferente['nombre']) ?>
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
