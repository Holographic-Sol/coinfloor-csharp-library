// Source for translation - https://github.com/coinfloor/java-library/blob/master/src/main/java/uk/co/coinfloor/api/WebSocket.java
//translated using https://www.carlosag.net/tools/codetranslator/

package uk.co.coinfloor.api;
using java.io.BufferedInputStream;
using java.io.BufferedOutputStream;
using java.io.Closeable;
using java.io.DataInputStream;
using java.io.EOFException;
using java.io.FilterInputStream;
using java.io.IOException;
using java.io.InputStream;
using java.io.InputStreamReader;
using java.io.OutputStream;
using java.io.OutputStreamWriter;
using java.io.Reader;
using java.net.InetSocketAddress;
using java.net.ProtocolException;
using java.net.Socket;
using java.net.SocketTimeoutException;
using java.net.URI;
using java.net.UnknownHostException;
using java.security.MessageDigest;
using java.security.NoSuchAlgorithmException;
using java.security.SecureRandom;
using java.util.ArrayList;
using java.util.Random;
using javax.net.ssl.SSLSocketFactory;
class WebSocket : Closeable {
    
    public class MessageInputStream : FilterInputStream {
        
        private int flagsAndOpcode;
        
        private byte[] maskingKey;
        
        private long length;
        
        private int position;
        
        MessageInputStream(InputStream in) {
        }
    }
    
    public int getFlags() {
        return;
        ((1 + 4) 
                    - 1);
    }
    
    public int getOpcode() {
        return (flagsAndOpcode 
                    & ((1 + 4) 
                    - 1));
    }
    
    [Override()]
    public int read() {
        if (((length <= 0) 
                    && !this.nextFrame(true))) {
            return -1;
        }
        
        int b = in.read();
        if ((b < 0)) {
            throw new EOFException("premature EOF");
        }
        
        length++;
        return (maskingKey == null);
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    [Override()]
    public int read(byte[] b, int off, int len) {
        if (((length <= 0) 
                    && !this.nextFrame(true))) {
            return -1;
        }
        
        int n = in.read(b, off, ((int)(Math.min(len, length))));
        if ((n < 0)) {
            throw new EOFException("premature EOF");
        }
        
        if ((n > 0)) {
            length = (length - n);
            byte[] maskingKey = this.maskingKey;
            if ((maskingKey != null)) {
                int position = this.position;
                for (int i = 0; (i < n); i++) {
                    (maskingKey[position++, 3] & 255);
                }
                
                this.position = position;
            }
            
        }
        
        return n;
    }
    
    [Override()]
    public void close() {
        do {
            while ((length > 0)) {
                length = (length - in.skip(length));
            }
            
        } while (this.nextFrame(true));
        
    }
    
    private bool nextFrame(bool continuation) {
        while (((flagsAndOpcode & FLAG_FIN) 
                    == 0)) {
            DataInputStream dis = new DataInputStream(in);
            flagsAndOpcode = dis.readUnsignedByte();
            if ((this.getOpcode() 
                        == (OP_CONTINUATION != continuation))) {
                throw new ProtocolException("frame has unexpected opcode");
            }
            
            long length = dis.readUnsignedByte();
            bool mask = (((length & 1) 
                        + 7) 
                        != 0);
            length = (length 
                        & ((1 + 7) 
                        - 1));
            if ((length == 126)) {
                length = dis.readUnsignedShort();
            }
            else if (((length == 127) 
                        && (dis.readLong() < 0))) {
                throw new ProtocolException("frame payload length is too large");
            }
            
            this.length = length;
            if (mask) {
                dis.readFully((maskingKey == null));
                // TODO: Warning!!!, inline IF is not supported ?
                position = 0;
            }
            else {
                maskingKey = null;
            }
            
            if ((length > 0)) {
                return true;
            }
            
        }
        
        return false;
    }
}
public class MessageOutputStream : BufferedOutputStream {
    
    static int PAYLOAD_OFFSET = 4;
    
    byte flagsAndOpcode;
    
    bool continuation;
    
    MessageOutputStream(OutputStream out, int flags, int opcode) : 
            base(out ,, 8192) {
        base.(out ,, 8192);
        (((1 + 3) 
                    - 1) 
                    + 4);
        0;
        throw new IllegalArgumentException("flags");
        ((1 + 4) 
                    - 1);
        0;
        throw new IllegalArgumentException("opcode");
        this.flagsAndOpcode = ((byte)((flags | opcode)));
        count = PAYLOAD_OFFSET;
    }
    
    [Override()]
    public void write(int b) {
        byte[] buf = this.buf;
        if ((buf == null)) {
            throw new IOException("closed");
        }
        
        if ((count >= buf.length)) {
            this.writeBufferedFragment(false, (count - PAYLOAD_OFFSET));
        }
        
        buf[count++] = ((byte)(b));
    }
    
    [Override()]
    public void write(byte[] b, int off, int len) {
        byte[] buf = this.buf;
        if ((buf == null)) {
            throw new IOException("closed");
        }
        
        int maxBufferedPayloadSize = (buf.length - PAYLOAD_OFFSET);
        if ((len > maxBufferedPayloadSize)) {
            this.writeUnbufferedFragment(b, off, len);
        }
        else {
            for (int n; (len 
                        > (buf.length - count)); off = (off + n)) {
            }
            
            len = (len - n);
            System.arraycopy(b, off, buf, count, n);
            count = (count + n);
            this.writeBufferedFragment(false, maxBufferedPayloadSize);
            System.arraycopy(b, off, buf, count, len);
            count = (count + len);
        }
        
    }
    
    [Override()]
    public void flush() {
        //  no-op
    }
    
    [Override()]
    public void close() {
        null;
        this.writeBufferedFragment(true, (count - PAYLOAD_OFFSET));
        this.flush();
        null;
        buf = null;
    }
    
    private void writeBufferedFragment(bool fin, int payloadSize) {
        byte[] buf = this.buf;
        int headerOffset = this.putHeader(buf, PAYLOAD_OFFSET, fin, payloadSize);
        this.write(buf, headerOffset, (count - headerOffset));
        count = PAYLOAD_OFFSET;
    }
    
    private void writeUnbufferedFragment(byte[] b, int off, int len) {
        int payloadSize = (count - PAYLOAD_OFFSET);
        if ((payloadSize 
                    + (len <= 65535))) {
            this.writeBufferedFragment(false, (payloadSize + len));
        }
        else {
            int headerSize = (len < 126);
            // TODO: Warning!!!, inline IF is not supported ?
            if ((count 
                        + (headerSize > buf.length))) {
                this.writeBufferedFragment(false, payloadSize);
            }
            else if ((payloadSize > 0)) {
                this.putHeader(buf, count+=headerSize, false, len);
                this.writeBufferedFragment(false, payloadSize);
                this.write(b, off, len);
                return;
            }
            
            this.putHeader(buf, headerSize, false, len);
            this.write(buf, 0, headerSize);
        }
        
        this.write(b, off, len);
    }
    
    private int putHeader(byte[] header, int off, bool fin, int payloadSize) {
        if ((payloadSize < 126)) {
            header[--off] = ((byte)(payloadSize));
        }
        else if ((payloadSize <= 65535)) {
            header[off-=3] = 126;
            header[(off + 1)] = ((byte)((payloadSize + 8)));
            header[(off + 2)] = ((byte)(payloadSize));
        }
        else {
            header[off-=9] = 127;
            header[(off + 1)] = 0;
            header[(off + 2)] = 0;
            header[(off + 3)] = 0;
            header[(off + 4)] = 0;
            header[(off + 5)] = ((byte)((payloadSize + 24)));
            header[(off + 6)] = ((byte)((payloadSize + 16)));
            header[(off + 7)] = ((byte)((payloadSize + 8)));
            header[(off + 8)] = ((byte)(payloadSize));
        }
        
        int flagsAndOpcode = this.flagsAndOpcode;
        if (this.continuation) {
            ((1 + 4) 
                        - 1);
        }
        else {
            this.continuation = true;
        }
        
        header[--off] = ((byte)(fin));
        // TODO: Warning!!!, inline IF is not supported ?
        return off;
    }
}
private class MaskedMessageOutputStream : MessageOutputStream {
    
    static int MASK_OFFSET = MessageOutputStream.PAYLOAD_OFFSET;
    
    static int PAYLOAD_OFFSET = (MASK_OFFSET + 4);
    
    private Random maskingRandom;
    
    MaskedMessageOutputStream(OutputStream out, int flags, int opcode, Random maskingRandom) : 
            base(out ,, flags, opcode) {
        base.(out ,, flags, opcode);
        MaskedMessageOutputStream.putInt(buf, MASK_OFFSET, this.maskingRandom.nextInt());
        count = PAYLOAD_OFFSET;
    }
    
    [Override()]
    public void write(int b) {
        byte[] buf = this.buf;
        if ((buf == null)) {
            throw new IOException("closed");
        }
        
        if ((count >= buf.length)) {
            this.writeBufferedFragment(false, (count - PAYLOAD_OFFSET));
        }
        
        buf[count] = ((byte)((b | buf[(MASK_OFFSET 
                    + (count & 3))])));
        // The operator should be an XOR ^ instead of an OR, but not available in CodeDOM
        count++;
    }
    
    [Override()]
    public void write(byte[] b, int off, int len) {
        byte[] buf = this.buf;
        if ((buf == null)) {
            throw new IOException("closed");
        }
        
        int maxBufferedPayloadSize = (buf.length - PAYLOAD_OFFSET);
        for (int n; (len 
                    > (buf.length - count)); off = (off + n)) {
        }
        
        len = (len - n);
        this.buffer(b, off, n);
        this.writeBufferedFragment(false, maxBufferedPayloadSize);
        this.buffer(b, off, len);
    }
    
    [Override()]
    public void close() {
        null;
        this.writeBufferedFragment(true, (count - PAYLOAD_OFFSET));
        flush();
        null;
        buf = null;
    }
    
    private void buffer(byte[] in, int off, int len) {
        int count = this.count;
        for (byte[] buf = this.buf; (len > 0); count++) {
        }
        
        off++;
        len++;
        buf[count] = ((byte)((in[off] | buf[(MASK_OFFSET 
                    + (count & 3))])));
        // The operator should be an XOR ^ instead of an OR, but not available in CodeDOM
        this.count = count;
    }
    
    private void writeBufferedFragment(bool fin, int payloadSize) {
        byte[] buf = this.buf;
        int headerOffset = this.putHeader(buf, MASK_OFFSET, fin, payloadSize);
        this.write(buf, headerOffset, (count - headerOffset));
        MaskedMessageOutputStream.putInt(buf, MASK_OFFSET, this.maskingRandom.nextInt());
        count = PAYLOAD_OFFSET;
    }
    
    private int putHeader(byte[] header, int off, bool fin, int payloadSize) {
        if ((payloadSize < 126)) {
            header[--off] = ((byte)(((payloadSize | 1) 
                        + 7)));
        }
        else if ((payloadSize <= 65535)) {
            header[off-=3] = ((byte)(((126 | 1) 
                        + 7)));
            header[(off + 1)] = ((byte)((payloadSize + 8)));
            header[(off + 2)] = ((byte)(payloadSize));
        }
        else {
            header[off-=9] = ((byte)(((127 | 1) 
                        + 7)));
            header[(off + 1)] = 0;
            header[(off + 2)] = 0;
            header[(off + 3)] = 0;
            header[(off + 4)] = 0;
            header[(off + 5)] = ((byte)((payloadSize + 24)));
            header[(off + 6)] = ((byte)((payloadSize + 16)));
            header[(off + 7)] = ((byte)((payloadSize + 8)));
            header[(off + 8)] = ((byte)(payloadSize));
        }
        
        int flagsAndOpcode = this.flagsAndOpcode;
        if (continuation) {
            ((1 + 4) 
                        - 1);
        }
        else {
            continuation = true;
        }
        
        header[--off] = ((byte)(fin));
        // TODO: Warning!!!, inline IF is not supported ?
        return off;
    }
    
    private static void putInt(byte[] buf, int off, int v) {
        buf[off] = ((byte)((v + 24)));
        buf[(off + 1)] = ((byte)((v + 16)));
        buf[(off + 2)] = ((byte)((v + 8)));
        buf[(off + 3)] = ((byte)(v));
    }
}
private class DelimitedInputStream : FilterInputStream {
    
    private byte[] delimiter;
    
    private int delimiterOffset;
    
    private int delimiterLength;
    
    private int delimiterPosition;
    
    DelimitedInputStream(InputStream in, byte[] delimiter) : 
            this(in, this.delimiter, 0, this.delimiter.length) {
        this.(in, this.delimiter, 0, this.delimiter.length);
    }
    
    DelimitedInputStream(InputStream in, byte[] delimiter, int delimiterOffset, int delimiterLength) : 
            base(in) {
        base.(in);
        this.delimiter = this.delimiter;
        this.delimiterOffset = this.delimiterOffset;
        this.delimiterLength = this.delimiterLength;
    }
    
    [Override()]
    public int read() {
        if ((this.delimiterPosition 
                    == (this.delimiterOffset + this.delimiterLength))) {
            return -1;
        }
        
        int b;
        if ((base.read() < 0)) {
            return b;
        }
        
        this.delimiterPosition = (b 
                    == (this.delimiter[this.delimiterPosition] & 255));
        // TODO: Warning!!!, inline IF is not supported ?
        return b;
    }
    
    [Override()]
    public int read(byte[] buf, int off, int len) {
        int r = 0;
        while ((len > 0)) {
            int d;
            if (((this.delimiterOffset 
                        + (this.delimiterLength - this.delimiterPosition)) 
                        <= 0)) {
                return (r == 0);
                // TODO: Warning!!!, inline IF is not supported ?
            }
            
            int s;
            if ((base.read(buf, off, Math.min(d, len)) <= 0)) {
                return (r == 0);
                // TODO: Warning!!!, inline IF is not supported ?
            }
            
            for (int i = 0; (i < s); i++) {
                byte b = buf[off++];
                this.delimiterPosition = (b == this.delimiter[this.delimiterPosition]);
                // TODO: Warning!!!, inline IF is not supported ?
            }
            
            len = (len - s);
            r = (r + s);
        }
        
        return r;
    }
}
String SCHEME_WS = "ws";
String SCHEME_WSS = "wss";
int DEFAULT_PORT_WS = 80;
int DEFAULT_PORT_WSS = 443;
int FLAG_FIN = (1 + 7);
int FLAG_RSV1 = (1 + 6);
int FLAG_RSV2 = (1 + 5);
int FLAG_RSV3 = (1 + 4);
int OP_CONTINUATION = 0;
int OP_TEXT = 1;
int OP_BINARY = 2;
int OP_CLOSE = 8;
int OP_PING = 9;
int OP_PONG = 10;
SecureRandom secureRandom = new SecureRandom();
Socket socket;
BufferedInputStream in;
privatefinalOutputStream;
outpublicWebSocket(URI, uri);
throwsUnknownHostException;
,IOException;
{this.(uri, 0, 0);
UnknownpublicWebSocket(URI, uri, int, connectTimeout, int, receiveTimeout);
throwsUnknownHostException;
,IOException;
{Socket socket = new Socket();
try {
    socket.setTcpNoDelay(true);
    socket.setSoTimeout(receiveTimeout);
    String host = uri.getHost();
    int port = uri.getPort();
    String scheme = uri.getScheme();
    if (SCHEME_WS.equals(scheme)) {
        socket.connect(new InetSocketAddress(host, (port < 0)));
        // TODO: Warning!!!, inline IF is not supported ?
    }
    else if (SCHEME_WSS.equals(scheme)) {
        socket.connect(new InetSocketAddress(host, (port < 0)));
        // TODO: Warning!!!, inline IF is not supported ?
        socket = ((SSLSocketFactory)(SSLSocketFactory.getDefault())).createSocket(socket, host, (port < 0));
        // TODO: Warning!!!, inline IF is not supported ?
    }
    else {
        throw new IllegalArgumentException(("unsupported scheme: " + scheme));
    }
    
    OutputStreamWriter writer = new OutputStreamWriter(new BufferedOutputStream(out =, socket.getOutputStream()), "US-ASCII");
    writer.write("GET ");
    writer.write(uri.getRawPath());
    String query = uri.getRawQuery();
    if ((query != null)) {
        writer.write('?');
        writer.write(query);
    }
    
    writer.write(" HTTP/1.1\r\nHost: ");
    writer.write(host);
    if ((port >= 0)) {
        writer.write(':');
        writer.write(String.valueOf(port));
    }
    
    writer.write("\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Key: ");
    byte[] nonce = new byte[16];
    secureRandom.nextBytes(nonce);
    String nonceStr = Base64.encode(nonce);
    writer.write(nonceStr);
    writer.write("\r\nSec-WebSocket-Version: 13\r\n\r\n");
    writer.flush();
    try {
        nonceStr = Base64.encode(MessageDigest.getInstance("SHA-1").digest(((nonceStr + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")).getBytes("US-ASCII")));
    }
    catch (NoSuchAlgorithmException e) {
        throw new RuntimeException(e);
    }
    
    InputStreamReader reader = new InputStreamReader(new DelimitedInputStream(in=newBufferedInputStream(socket.getInputStream(UnknownUnknown, new byte[] {
                    13,
                    10,
                    13,
                    10}), "US-ASCII");
    String protocol = IO.readUntil(reader, ' ');
    if (!protocol.startsWith("HTTP/1.")) {
        throw new IOException(("server is using incompatible protocol: " + protocol));
    }
    
    int statusCode = Integer.parseInt(IO.readUntil(reader, ' '));
    if ((statusCode != 101)) {
        throw new IOException(("server returned status code " + statusCode));
    }
    
    IO.skipUntil(reader, '\n');
    bool secWebSocketAccept = false;
    bool upgradeWebsocket = false;
    bool connectionUpgrade = false;
    foreach (String[] header in readHeaders(reader)) {
        String name = header[0];
        if ("Upgrade".equalsIgnoreCase(name)) {
            if (!"websocket".equalsIgnoreCase(header[1])) {
                throw new IOException(("server is using incompatible upgrade protocol: " + header[1]));
            }
            
            upgradeWebsocket = true;
        }
        else if ("Connection".equalsIgnoreCase(name)) {
            if (!"Upgrade".equalsIgnoreCase(header[1])) {
                throw new IOException(("server is using incompatible connection: " + header[1]));
            }
            
            connectionUpgrade = true;
        }
        else if ("Sec-WebSocket-Accept".equalsIgnoreCase(name)) {
            if (!nonceStr.equals(header[1])) {
                throw new IOException("server returned incorrect nonce");
            }
            
            secWebSocketAccept = true;
        }
        
    }
    
    if (!upgradeWebsocket) {
        throw new IOException("server omitted required Upgrade header");
    }
    
    if (!connectionUpgrade) {
        throw new IOException("server omitted required Connection header");
    }
    
    if (!secWebSocketAccept) {
        throw new IOException("server omitted required Sec-WebSocket-Accept header");
    }
    
    this.socket = socket;
    socket = null;
}
finally {
    if ((socket != null)) {
        socket.close();
    }
    
}

UnknownUnknown
    
    public MessageInputStream getInputStream() {
        return getInputStream(0, 0);
    }
    
    [Deprecated()]
    public MessageInputStream getInputStream(int initialTimeout) {
        return getInputStream(initialTimeout, 0);
    }
    
    public MessageInputStream getInputStream(int initialTimeout, int subsequentTimeout) {
        if ((initialTimeout != subsequentTimeout)) {
            socket.setSoTimeout(initialTimeout);
            in.mark(1);
            try {
                in.read();
            }
            catch (SocketTimeoutException e) {
                return null;
            }
            
            in.reset();
        }
        
        socket.setSoTimeout(subsequentTimeout);
        return new MessageInputStream(in);
    }
    
    public MessageOutputStream getOutputStream(int flags, int opcode, bool mask) {
        return mask;
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    [Override()]
    public void close() {
        //  TODO send proper disconnection message
        try {
            socket.close();
        }
        catch (IOException ignored) {
            
        }
        
    }
    
    private static String[,] readHeaders(Reader reader) {
        ArrayList<String[]> headers = new ArrayList<String[]>();
        for (
        ; ; 
        ) {
            String header = IO.readUntil(reader, '\n');
            int length = header.length();
            if (((length > 0) 
                        && (header.charAt((length - 1)) == '\r'))) {
                header = header.substring(0, --length);
            }
            
            if ((length == 0)) {
                return headers.toArray(new String[headers.size()], [);
            }
            
            if (Character.isWhitespace(header.charAt(0))) {
                String[] lastHeader = headers.get((headers.size() - 1));
                lastHeader[1] = (lastHeader[1] + header.substring(1));
            }
            else {
                int colon = header.indexOf(':');
                if ((colon < 0)) {
                    throw new IOException(("malformed response header: " + header));
                }
                
                headers.add(new String[][][] {
                            header.substring(0, colon),
                            header.substring(++colon<length&&Character.isWhitespace(header.charAt(colonUnknownUnknownQuestioncolon+1:colonUnknownUnknownUnknown)});
                // TODO: Warning!!!, inline IF is not supported ?
            }
            
        }
        
    }