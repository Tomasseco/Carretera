# Servidor 

Este proyecto consiste en dos aplicaciones en C#: un **Servidor** y varios **Clientes** (vehículos) que se conectan a él.

---

# README - Servidor 🖥️

## ¿Qué hace el servidor?

El servidor se encarga de esperar a que lleguen vehículos (clientes), les asigna un número de identificación (ID) y hace un pequeño "saludo" para asegurarse de que están listos para comunicarse.

## Pasos que sigue el servidor

### 1. Poner en marcha el servidor

```csharp
listener = new TcpListener(IPAddress.Any, 5000);
listener.Start();
```

- Creamos un `TcpListener` para estar atentos a quien quiera conectarse en el puerto 5000.
- `IPAddress.Any` permite aceptar conexiones desde cualquier parte.

## Así el servidor queda "escuchando" para cualquier cliente que quiera entrar.

### 2. Aceptar clientes mientras siguen llegando

```csharp
TcpClient clienteTcp = listener.AcceptTcpClient();
Thread hilo = new Thread(() => GestionarCliente(clienteTcp));
hilo.Start();
```

- Cada vez que alguien se conecta, creamos un nuevo hilo para encargarnos de él, así evitando que se bloquee las peticiones

## Así el servidor puede seguir atendiendo a nuevos vehículos mientras habla con los que ya están conectados.

### 3. Asignar ID a cada cliente

```csharp
lock (lockClientes)
{
    idAsignado = ++contadorIds;
    Cliente nuevoCliente = new Cliente(idAsignado, ns);
    clientesConectados.Add(nuevoCliente);
}
```

- Protegemos el acceso a la lista de clientes con LOCK para evitar líos si llegan varios al mismo tiempo.
- Vamos generando IDs únicos para cada cliente que se conecta.

## ¡Cada vehículo tiene su propio número para que sepamos quién es quién!

### 4. Handshake: El "apretón de manos"

- El cliente manda un "INICIO".
- El servidor responde enviándole su ID.
- El cliente le devuelve ese mismo ID para confirmar.

## Así nos aseguramos de que todo está en orden antes de empezar a enviar mensajes serios.

### 5. Clase Cliente (archivo externo)

```csharp
public class Cliente
{
    public int Id { get; set; }
    public NetworkStream Stream { get; set; }
}
```