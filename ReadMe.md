# OnlineMsgServer

## 项目介绍
Online Message Server 在线消息服务器，使用非对称加密确保安全发送消息

## 使用 Docker 运行

### 步骤
1. 进入项目目录：
    ```bash
    cd OnlineMsgServer
    ```

2. 构建镜像：
    ```bash
    docker build -t onlinemsgserver .
    ```

3. 启动容器：
    ```bash
    docker run -d --rm -p 13173:13173 onlinemsgserver
    ```

4. 使用客户端连接服务器进行通信

## 通信规则

### 消息加密方式
使用RSA-2048-OAEP-256非对称加密，消息需分块（块大小190字节）进行加密，按顺序合并后编码成base64字符串进行发送。

收到密文后需从base64解码，然后按照块大小256字节进行分块解密，按顺序合并后得到原文。

使用服务器公钥加密消息发送给服务器，然后使用自己的私钥解密从服务器收到的消息。

### Message结构
```json
{
  "type": "publickey|forward|broadcast",
  "key": "",
  "data": ""
}
```
type为publickey发送给服务器时，data为交换的公钥（base64字符串格式），key为自己的名字（也可为空，服务器会将其视作匿名用户）

type为forward且发送给服务器时，key为需要服务器转发给的对象公钥，data为转发内容。从服务器收到type为forward的消息时，key为消息实际发送者，data为转发内容。

type为broadcast且发送给服务器时，key可空，data为广播内容。从服务器收到type为broadcast的消息时，key为消息实际发送者名称，data为广播内容。

### 交互过程
1. 客户端第一次和服务器建立连接时，服务器即会返回一个**没有加密**的publickey消息，data为服务器公钥
   ```json
    {
        "type": "publickey",
        "key": "XNoZGtsamFoc2xrZGpoYXNrbGQ=..."
    }
    ```
2. 客户端将自己的公钥加密发送给服务器，视作登记，只有登记的客户端才可以收到服务器消息

3. 客户端发送forward和broadcast消息来操作服务器是转发data中的内容还是广播data中的内容


