# Proyecto Cliente - Simulación de Vehículos Conectados

Este README te explica de forma fácil y rápida cómo funciona el programa Cliente que se conecta al Servidor.

---

# Cliente - Objetivo General

El Cliente simula un vehículo que:

- Se conecta al servidor.
- Hace un "handshake" para recibir su ID único.
- Queda listo para enviar o recibir información.

Todo pensado para que cada vehículo pueda ser controlado y localizado por el servidor.

---

# Pasos principales del Cliente

## 1. Conexión al servidor

```csharp
TcpClient cliente = new TcpClient("127.0.0.1", 5000);
```

- El Cliente abre una conexión al servidor que está escuchando en el puerto **5000**.
- Nos conectamos a **localhost** (o IP local) para pruebas.

¿Por qué? Así simulamos muchos vehículos conectándose de manera local primero.

---

## 2. Obtener el NetworkStream

```csharp
NetworkStream ns = cliente.GetStream();
```

- Usamos `NetworkStream` para enviar y recibir mensajes fácilmente.

¿Por qué? Simplifica el trabajo de comunicación.

---

## 3. Handshake inicial

El Cliente debe hablar con el servidor para establecer la comunicación:

1. El Cliente envía el mensaje **"INICIO"**.
2. El Servidor responde enviándole un **ID único**.
3. El Cliente responde enviando otra vez el mismo ID como confirmación.

Todo esto usando:

```csharp
NetworkStreamClass.EscribirMensajeNetworkStream(ns, "INICIO");
string idRecibido = NetworkStreamClass.LeerMensajeNetworkStream(ns);
NetworkStreamClass.EscribirMensajeNetworkStream(ns, idRecibido);
```

¿Por qué? Para asegurarnos que el servidor sabe exactamente quién es cada cliente y que está todo bien sincronizado.

---

# Cómo usamos `NetworkStreamClass`

Para no repetir siempre el mismo código, tenemos los métodos:

- `EscribirMensajeNetworkStream`: Envia un string por el `NetworkStream`.
- `LeerMensajeNetworkStream`: Recibe un string desde el `NetworkStream`.


