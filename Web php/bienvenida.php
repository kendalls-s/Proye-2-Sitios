<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header('Location: index.php');
    exit;
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Administración de Personal</title>
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

        .bienvenida-info {
            background: #ffffff;
            border-radius: 8px;
            box-shadow: 0 3px 10px rgba(0, 0, 0, .08);
            padding: 32px;
            max-width: 600px;
            margin-top: 32px;
        }

        .bienvenida-info p {
            color: #666;
            font-size: 16px;
            line-height: 1.6;
            margin: 12px 0;
        }

        .bienvenida-info .hint {
            color: #2563eb;
            font-weight: bold;
            margin-top: 20px;
            padding-top: 20px;
            border-top: 1px solid #e5e7eb;
        }
    </style>
</head>
<body>
    <header>
        <span class="titulo">Administración de Personal - Servicios Médicos SA</span>
        <nav>
            <a href="puestos.php" class="btn-puestos">Puestos</a>
            <a href="logout.php">Cerrar sesión</a>
        </nav>
    </header>

    <main>
        <h1>Bienvenido, <?= htmlspecialchars($_SESSION['nombreCompleto'] ?? $_SESSION['usuario']) ?></h1>

        <div class="bienvenida-info">
            <p>Administración de Personal - Servicios Médicos SA</p>
            <p>Acceda al módulo de gestión haciendo clic en el botón <strong>"Puestos"</strong> en la barra de navegación superior.</p>
            <div class="hint">👉 Desde allí podrá consultar puestos disponibles, oferentes y crear nuevos empleados.</div>
        </div>
    </main>
</body>
</html>
