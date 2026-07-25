<?php
session_start();
require_once __DIR__ . '/data.php';

$error = '';

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $usuario = trim($_POST['usuario'] ?? '');
    $password = $_POST['password'] ?? '';

    if ($usuario === '' || $password === '') {
        $error = 'Usuario y/o contraseña incorrectos.';
    } else {
        $resultado = autenticarWCF($usuario, $password);

        if ($resultado['autenticado']) {
            $_SESSION['usuario'] = $usuario;
            $_SESSION['nombreCompleto'] = $resultado['nombreCompleto'];
            header('Location: bienvenida.php');
            exit;
        }

        $error = $resultado['mensaje'] !== '' ? $resultado['mensaje'] : 'Usuario y/o contraseña incorrectos.';
    }
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Ingreso al sistema</title>
    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background-color: #f2f4f7;
            display: flex;
            align-items: center;
            justify-content: center;
            height: 100vh;
        }

        .login-box {
            background-color: #ffffff;
            width: 380px;
            border: 3px solid #1b3a63;
            border-radius: 16px;
            padding: 32px;
            box-sizing: border-box;
        }

        h1 {
            margin: 0;
            text-align: center;
            font-size: 22px;
            color: #1a1a1a;
        }

        .subtitulo {
            text-align: center;
            color: #8a8f98;
            margin: 4px 0 24px;
            font-size: 14px;
        }

        .error {
            background-color: #fbe4e4;
            color: #a33;
            border: 1px solid #f0c4c4;
            border-radius: 6px;
            padding: 12px;
            font-size: 14px;
            margin-bottom: 20px;
        }

        label {
            display: block;
            margin-bottom: 6px;
            color: #1a1a1a;
            font-size: 14px;
        }

        .campo {
            display: flex;
            align-items: center;
            border: 1px solid #d9dce1;
            border-radius: 6px;
            margin-bottom: 18px;
            overflow: hidden;
        }

        .campo .icono {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 42px;
            height: 42px;
            background-color: #f5f6f8;
            color: #8a8f98;
        }

        .campo input {
            border: none;
            outline: none;
            flex: 1;
            padding: 10px 12px;
            font-size: 14px;
        }

        .campo:focus-within {
            border-color: #3b82f6;
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
        }

        button {
            width: 100%;
            padding: 12px;
            background-color: #2563eb;
            color: #ffffff;
            border: none;
            border-radius: 6px;
            font-size: 15px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
        }

        button:hover {
            background-color: #1d4ed8;
        }
    </style>
</head>
<body>
    <div class="login-box">
        <h1>Administración de Personal</h1>
        <p class="subtitulo">Servicios Médicos SA</p>

        <?php if ($error !== ''): ?>
            <div class="error"><?= htmlspecialchars($error) ?></div>
        <?php endif; ?>

        <form method="post" action="">
            <label for="usuario">Usuario</label>
            <div class="campo">
                <span class="icono">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 4-6 8-6s8 2 8 6"/></svg>
                </span>
                <input type="text" id="usuario" name="usuario" value="<?= htmlspecialchars($_POST['usuario'] ?? '') ?>" required>
            </div>

            <label for="password">Contraseña</label>
            <div class="campo">
                <span class="icono">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="4" y="11" width="16" height="9" rx="2"/><path d="M8 11V7a4 4 0 0 1 8 0v4"/></svg>
                </span>
                <input type="password" id="password" name="password" required>
            </div>

            <button type="submit">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"/><path d="M10 17l5-5-5-5"/><path d="M15 12H3"/></svg>
                Aceptar
            </button>
        </form>
    </div>
</body>
</html>
