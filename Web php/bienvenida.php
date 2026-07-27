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
            margin-bottom: 32px;
        }

        .modulos-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 24px;
            max-width: 1200px;
        }

        .tarjeta-modulo {
            background: #ffffff;
            border-radius: 8px;
            box-shadow: 0 3px 10px rgba(0, 0, 0, .08);
            padding: 24px;
            transition: all 0.3s ease;
            text-decoration: none;
            color: #333;
            border-left: 4px solid #2563eb;
        }

        .tarjeta-modulo:hover {
            box-shadow: 0 6px 20px rgba(0, 0, 0, .12);
            transform: translateY(-2px);
        }

        .tarjeta-modulo h2 {
            margin-top: 0;
            color: #1b3a63;
            font-size: 20px;
        }

        .tarjeta-modulo p {
            color: #666;
            font-size: 14px;
            line-height: 1.5;
            margin: 12px 0;
        }

        .etiqueta-core {
            display: inline-block;
            background-color: #e3f2fd;
            color: #1565c0;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: bold;
            margin-bottom: 8px;
        }

        .enlace-acceso {
            display: inline-block;
            background-color: #2563eb;
            color: white;
            padding: 10px 16px;
            border-radius: 4px;
            text-decoration: none;
            font-weight: bold;
            margin-top: 12px;
            transition: background-color 0.3s ease;
        }

        .enlace-acceso:hover {
            background-color: #1e40af;
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

        <div class="modulos-grid">
            <!-- Core1 & Core3: Gestión de Puestos y Empleados -->
            <a href="puestos.php" class="tarjeta-modulo">
                <span class="etiqueta-core">Core 1, 3, 6, 9</span>
                <h2>Gestión de Personal</h2>
                <p>Accede al listado de puestos activos, revisa los oferentes disponibles y crea nuevos empleados.</p>
                <div class="enlace-acceso">Ir al módulo →</div>
            </a>
        </div>
    </main>
</body>
</html>
