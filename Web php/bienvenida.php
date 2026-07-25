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
    </style>
</head>
<body>
    <header>
        <span>Administración de Personal - Servicios Médicos SA</span>
        <a href="logout.php">Cerrar sesión</a>
    </header>

    <main>
        <h1>Bienvenido, <?= htmlspecialchars($_SESSION['nombreCompleto'] ?? $_SESSION['usuario']) ?></h1>
    </main>
</body>
</html>
