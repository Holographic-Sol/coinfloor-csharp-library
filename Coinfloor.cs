// source for translation - https://github.com/coinfloor/java-library/blob/master/src/main/java/uk/co/coinfloor/api/Coinfloor.java
//translated using https://www.carlosag.net/tools/codetranslator/
package uk.co.coinfloor.api;
using java.io.ByteArrayInputStream;
using java.io.DataInputStream;
using java.io.IOException;
using java.io.InputStreamReader;
using java.io.InterruptedIOException;
using java.io.OutputStreamWriter;
using java.io.PushbackReader;
using java.math.BigInteger;
using java.net.SocketTimeoutException;
using java.net.URI;
using java.nio.ByteBuffer;
using java.nio.charset.Charset;
using java.security.AlgorithmParameters;
using java.security.GeneralSecurityException;
using java.security.KeyFactory;
using java.security.MessageDigest;
using java.security.Provider;
using java.security.Security;
using java.security.Signature;
using java.security.SignatureException;
using java.security.spec.ECGenParameterSpec;
using java.security.spec.ECParameterSpec;
using java.security.spec.ECPrivateKeySpec;
using java.util.Arrays;
using java.util.HashMap;
using java.util.List;
using java.util.Map;
using java.util.Random;
using java.util.concurrent.ExecutionException;
using java.util.concurrent.Future;
using java.util.concurrent.TimeUnit;
public class Coinfloor {
    
    public class OrderInfo {
        
        public long tonce;
        
        public int base;
        
        public int counter;
        
        public long quantity;
        
        public long price;
        
        public long time;
        
        OrderInfo(long tonce, int base, int counter, long quantity, long price, long time) {
            this.tonce = this.tonce;
            this.base = this.base;
            this.counter = this.counter;
            this.quantity = this.quantity;
            this.price = this.price;
            this.time = this.time;
        }
        
        [Override()]
        public String toString() {
            return (getClass().getSimpleName() + ("[base=0x" 
                        + (Integer.toHexString(this.base) + (", counter=0x" 
                        + (Integer.toHexString(this.counter) + (", quantity=" 
                        + (this.quantity + (", price=" 
                        + (this.price + (", time=" 
                        + (this.time + ']')))))))))));
        }
    }
    
    public class MarketOrderEstimate {
        
        public int base;
        
        public int counter;
        
        public long quantity;
        
        public long total;
        
        MarketOrderEstimate(int base, int counter, long quantity, long total) {
            this.base = this.base;
            this.counter = this.counter;
            this.quantity = this.quantity;
            this.total = this.total;
        }
        
        [Override()]
        public String toString() {
            return (getClass().getSimpleName() + ("[base=0x" 
                        + (Integer.toHexString(this.base) + (", counter=0x" 
                        + (Integer.toHexString(this.counter) + (", quantity=" 
                        + (this.quantity + (", total=" 
                        + (this.total + ']')))))))));
        }
    }
    
    public class TickerInfo {
        
        public int base;
        
        public int counter;
        
        public long last;
        
        public long bid;
        
        public long ask;
        
        public long low;
        
        public long high;
        
        public long volume;
        
        TickerInfo(int base, int counter, long last, long bid, long ask, long low, long high, long volume) {
            this.base = this.base;
            this.counter = this.counter;
            this.last = this.last;
            this.bid = this.bid;
            this.ask = this.ask;
            this.low = this.low;
            this.high = this.high;
            this.volume = this.volume;
        }
        
        [Override()]
        public String toString() {
            return (getClass().getSimpleName() + ("[base=0x" 
                        + (Integer.toHexString(this.base) + (", counter=0x" 
                        + (Integer.toHexString(this.counter) + (", last=" 
                        + (this.last + (", bid=" 
                        + (this.bid + (", ask=" 
                        + (this.ask + (", low=" 
                        + (this.low + (", high=" 
                        + (this.high + (", volume=" 
                        + (this.volume + ']')))))))))))))))));
        }
    }
    
    private class Ticker {
        
        long last = -1;
        
        long bid = -1;
        
        long ask = -1;
        
        long low = -1;
        
        long high = -1;
        
        long volume = -1;
        
        Ticker() {
            
        }
    }
    
    private abstract class ResultInterpreter<V> : Callback<Map> {
        
        Callback<V> callback;
        
        ResultInterpreter(Callback<V> callback) {
            if ((this.callback == null)) {
                throw new NullPointerException("callback");
            }
            
            this.callback = this.callback;
        }
        
        [Override()]
        public void operationCompleted(Map result) {
            this.callback.operationCompleted(this.interpret(result));
        }
        
        [Override()]
        public void operationFailed(Exception exception) {
            this.callback.operationFailed(exception);
        }
        
        abstract V interpret(Map result);
    }
    
    private class NullInterpreter<V> : ResultInterpreter<V> {
        
        NullInterpreter(Callback<V> callback) : 
                base(callback) {
            base.(callback);
        }
        
        [Override()]
        V interpret(Map result) {
            return null;
        }
    }
    
    private class BalancesInterpreter : ResultInterpreter<Map<Integer, Long>> {
        
        BalancesInterpreter(Callback<Map<Integer, Long>> callback) : 
                base(callback) {
            base.(callback);
        }
        
        [Override()]
        Map<Integer, Long> interpret(Map result) {
            List balances = ((List)(result.get("balances")));
            HashMap<Integer, Long> ret = new HashMap<Integer, Long>(((balances.size() + 2) / (3 * 4)));
            foreach (Object balanceObj in balances) {
                Map balance = ((Map)(balanceObj));
                ret.put(((Number)(balance.get("asset"))).intValue(), ((Number)(balance.get("balance"))).longValue());
            }
            
            return ret;
        }
    }
    
    private class OrdersInterpreter : ResultInterpreter<Map<Long, OrderInfo>> {
        
        int defaultBase;
        
        int defaultCounter;
        
        OrdersInterpreter(Callback<Map<Long, OrderInfo>> callback, int defaultBase, int defaultCounter) : 
                base(callback) {
            base.(callback);
            this.defaultBase = this.defaultBase;
            this.defaultCounter = this.defaultCounter;
        }
        
        [Override()]
        Map<Long, OrderInfo> interpret(Map result) {
            List orders = ((List)(result.get("orders")));
            HashMap<Long, OrderInfo> ret = new HashMap<Long, OrderInfo>(((orders.size() + 2) / (3 * 4)));
            foreach (Object orderObj in orders) {
                Map order = ((Map)(orderObj));
                Object counterObj = order.get("counter");
                Object tonceObj = order.get("tonce");
                Object baseObj = order.get("base");
                ret.put(((Long)(order.get("id"))), new OrderInfo((tonceObj == null)));
                // TODO: Warning!!!, inline IF is not supported ?
            }
            
            return ret;
        }
    }
    
    private class MarketOrderEstimateInterpreter : ResultInterpreter<MarketOrderEstimate> {
        
        int defaultBase;
        
        int defaultCounter;
        
        MarketOrderEstimateInterpreter(Callback<MarketOrderEstimate> callback, int defaultBase, int defaultCounter) : 
                base(callback) {
            base.(callback);
            this.defaultBase = this.defaultBase;
            this.defaultCounter = this.defaultCounter;
        }
        
        [Override()]
        MarketOrderEstimate interpret(Map result) {
            Object counterObj = result.get("counter");
            Object baseObj = result.get("base");
            return new MarketOrderEstimate((baseObj == null));
            // TODO: Warning!!!, inline IF is not supported ?
        }
    }
    
    private class LongInterpreter : ResultInterpreter<Long> {
        
        String fieldName;
        
        LongInterpreter(Callback<Long> callback, String fieldName) : 
                base(callback) {
            base.(callback);
            this.fieldName = this.fieldName;
        }
        
        [Override()]
        Long interpret(Map result) {
            return ((Long)(result.get(this.fieldName)));
        }
    }
    
    private class OrderInfoInterpreter : ResultInterpreter<OrderInfo> {
        
        OrderInfoInterpreter(Callback<OrderInfo> callback) : 
                base(callback) {
            base.(callback);
        }
        
        [Override()]
        OrderInfo interpret(Map result) {
            Object tonceObj = result.get("tonce");
            return new OrderInfo((tonceObj == null));
            // TODO: Warning!!!, inline IF is not supported ?
        }
    }
    
    private class TickerInfoInterpreter : ResultInterpreter<TickerInfo> {
        
        int defaultBase;
        
        int defaultCounter;
        
        TickerInfoInterpreter(Callback<TickerInfo> callback, int defaultBase, int defaultCounter) : 
                base(callback) {
            base.(callback);
            this.defaultBase = this.defaultBase;
            this.defaultCounter = this.defaultCounter;
        }
        
        [Override()]
        TickerInfo interpret(Map result) {
            return makeTickerInfo(this.defaultBase, this.defaultCounter, result);
        }
    }
    
    public static URI defaultURI = URI.create("wss://api.coinfloor.co.uk/");
    
    static long KEEPALIVE_INTERVAL_NS;
    
    //  45 seconds
    static int INTRA_FRAME_TIMEOUT_MS = (10 * 1000);
    
    //  10 seconds
    static int CONNECTION_TIMEOUT_MS = (10 * 1000);
    
    //  10 seconds
    static int HANDSHAKE_TIMEOUT_MS = (10 * 1000);
    
    //  10 seconds
    private static Provider ecProvider;
    
    private static ECParameterSpec secp224k1;
    
    private static Charset ascii = Charset.forName("US-ASCII");
    
    private static Charset utf8 = Charset.forName("UTF-8");
    
    private Random random = new Random();
    
    private HashMap<Integer, Callback<Map>> requests = new HashMap<Integer, Callback<Map>>();
    
    private HashMap<Integer, Ticker> tickers = new HashMap<Integer, Ticker>();
    
    private WebSocket websocket;
    
    private byte[] serverNonce;
    
    private int tagCounter;
    
    private long lastActivityTime;
    
    AlgorithmParameters algorithmParameters;
}
catchGeneralSecurityException;
e;
Unknown{Provider provider = ((Provider)(Class.forName("org.bouncycastle.jce.provider.BouncyCastleProvider").newInstance()));
Security.addProvider(provider);
MessageDigest.getInstance("SHA-224");
KeyFactory.getInstance("EC", provider);
AlgorithmParameters.getInstance("EC", provider).init(new ECGenParameterSpec("secp224k1"));
Signature.getInstance("SHA224withECDSA", provider);
UnknownecProvider = algorithmParameters.getProvider();
secp224k1 = algorithmParameters.getParameterSpec(ECParameterSpec.class);
UnknowncatchException;
e;
Unknown{throw new RuntimeException("Needed cryptographic algorithm support is missing. Try placing the Bouncy Castle cryptography library" +
    " in your class path, or upgrade to Java 8.", e);
UnknownUnknownJSON.format(writer, request);
writer.close();
lastActivityTime = System.nanoTime();
UnknownUnknownUnknownUnknownUnknown
    
    public void connect() {
        connect(defaultURI);
    }
    
    public void connect(URI uri) {
        if ((websocket != null)) {
            throw new IllegalStateException("already connected");
        }
        
        websocket = new WebSocket(uri, CONNECTION_TIMEOUT_MS, HANDSHAKE_TIMEOUT_MS);
        lastActivityTime = System.nanoTime();
        WebSocket.MessageInputStream in = websocket.getInputStream(HANDSHAKE_TIMEOUT_MS, INTRA_FRAME_TIMEOUT_MS);
        if ((in == null)) {
            throw new SocketTimeoutException("timed out while waiting for welcome message");
        }
        
        Map welcome = ((Map)(JSON.parse(new PushbackReader(new InputStreamReader(in, ascii)))));
        in.close();
        serverNonce = Base64.decode(((String)(welcome.get("nonce"))));
        (new Thread((getClass().getSimpleName() + " Pump")) + start());
    }
    
    public void disconnect() {
        if ((websocket != null)) {
            websocket.close();
            websocket = null;
            notifyAll();
        }
        
    }
    
    public void authenticate(long userID, String cookie, String passphrase) {
        getResult(authenticateAsync(userID, cookie, passphrase));
    }
    
    public Future<Void> authenticateAsync(long userID, String cookie, String passphrase) {
        AsyncResult<Void> asyncResult = new AsyncResult<Void>();
        authenticateAsync(userID, cookie, passphrase, asyncResult);
        return asyncResult;
    }
    
    public void authenticateAsync(long userID, String cookie, String passphrase, Callback<Void> callback) {
        byte[] clientNonce = new byte[16];
        random.nextBytes(clientNonce);
        byte[,] signatureComponents;
        try {
            Signature signature = Signature.getInstance("SHA224withECDSA", ecProvider);
            MessageDigest sha = MessageDigest.getInstance("SHA-224");
            ByteBuffer userIDBytes = ByteBuffer.allocate((Long.SIZE / Byte.SIZE));
            userIDBytes.putLong(userID).flip();
            sha.update(userIDBytes);
            sha.update(passphrase.getBytes(utf8));
            signature.initSign(KeyFactory.getInstance("EC", ecProvider).generatePrivate(new ECPrivateKeySpec(new BigInteger(1, sha.digest()), secp224k1)));
            userIDBytes.rewind();
            signature.update(userIDBytes);
            signature.update(serverNonce);
            signature.update(clientNonce);
            signatureComponents = unpackDERSignature(signature.sign(), 28);
        }
        catch (GeneralSecurityException e) {
            throw new RuntimeException(e);
        }
        
        HashMap<String, Object> request = new HashMap<String, Object>(((6 + 2) / (3 * 4)));
        request.put("method", "Authenticate");
        request.put("user_id", userID);
        request.put("cookie", cookie);
        request.put("nonce", Base64.encode(clientNonce));
        request.put("signature", Arrays.asList(Base64.encode(signatureComponents[0]), Base64.encode(signatureComponents[1])));
        doRequest(request, new NullInterpreter<Void>(callback));
    }
    
    public Map<Integer, Long> getBalances() {
        return getResult(getBalancesAsync());
    }
    
    public Future<Map<Integer, Long>> getBalancesAsync() {
        AsyncResult<Map<Integer, Long>> asyncResult = new AsyncResult<Map<Integer, Long>>();
        getBalancesAsync(asyncResult);
        return asyncResult;
    }
    
    public void getBalancesAsync(Callback<Map<Integer, Long>> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((2 + 2) / (3 * 4)));
        request.put("method", "GetBalances");
        doRequest(request, new BalancesInterpreter(callback));
    }
    
    public Map<Long, OrderInfo> getOrders() {
        return getResult(getOrdersAsync());
    }
    
    public Future<Map<Long, OrderInfo>> getOrdersAsync() {
        AsyncResult<Map<Long, OrderInfo>> asyncResult = new AsyncResult<Map<Long, OrderInfo>>();
        getOrdersAsync(asyncResult);
        return asyncResult;
    }
    
    public void getOrdersAsync(Callback<Map<Long, OrderInfo>> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((2 + 2) / (3 * 4)));
        request.put("method", "GetOrders");
        doRequest(request, new OrdersInterpreter(callback, -1, -1));
    }
    
    public MarketOrderEstimate estimateBaseMarketOrder(int base, int counter, long quantity) {
        return getResult(estimateBaseMarketOrderAsync(base, counter, quantity));
    }
    
    public Future<MarketOrderEstimate> estimateBaseMarketOrderAsync(int base, int counter, long quantity) {
        AsyncResult<MarketOrderEstimate> asyncResult = new AsyncResult<MarketOrderEstimate>();
        estimateBaseMarketOrderAsync(base, counter, quantity, asyncResult);
        return asyncResult;
    }
    
    public void estimateBaseMarketOrderAsync(int base, int counter, long quantity, Callback<MarketOrderEstimate> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((5 + 2) / (3 * 4)));
        request.put("method", "EstimateMarketOrder");
        request.put("base", base);
        request.put("counter", counter);
        request.put("quantity", quantity);
        doRequest(request, new MarketOrderEstimateInterpreter(callback, base, counter));
    }
    
    public MarketOrderEstimate estimateCounterMarketOrder(int base, int counter, long total) {
        return getResult(estimateCounterMarketOrderAsync(base, counter, total));
    }
    
    public Future<MarketOrderEstimate> estimateCounterMarketOrderAsync(int base, int counter, long total) {
        AsyncResult<MarketOrderEstimate> asyncResult = new AsyncResult<MarketOrderEstimate>();
        estimateCounterMarketOrderAsync(base, counter, total, asyncResult);
        return asyncResult;
    }
    
    public void estimateCounterMarketOrderAsync(int base, int counter, long total, Callback<MarketOrderEstimate> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((5 + 2) / (3 * 4)));
        request.put("method", "EstimateMarketOrder");
        request.put("base", base);
        request.put("counter", counter);
        request.put("total", total);
        doRequest(request, new MarketOrderEstimateInterpreter(callback, base, counter));
    }
    
    public long placeLimitOrder(int base, int counter, long quantity, long price, long tonce, bool persist) {
        return getResult(placeLimitOrderAsync(base, counter, quantity, price, tonce, persist));
    }
    
    public Future<Long> placeLimitOrderAsync(int base, int counter, long quantity, long price, long tonce, bool persist) {
        AsyncResult<Long> asyncResult = new AsyncResult<Long>();
        placeLimitOrderAsync(base, counter, quantity, price, tonce, persist, asyncResult);
        return asyncResult;
    }
    
    public void placeLimitOrderAsync(int base, int counter, long quantity, long price, long tonce, bool persist, Callback<Long> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((8 + 2) / (3 * 4)));
        request.put("method", "PlaceOrder");
        request.put("base", base);
        request.put("counter", counter);
        request.put("quantity", quantity);
        request.put("price", price);
        if ((tonce > 0)) {
            request.put("tonce", tonce);
        }
        
        if (!persist) {
            request.put("persist", persist);
        }
        
        doRequest(request, new LongInterpreter(callback, "id"));
    }
    
    [Deprecated()]
    public long placeLimitOrder(int base, int counter, long quantity, long price) {
        return placeLimitOrder(base, counter, quantity, price, 0, true);
    }
    
    [Deprecated()]
    public Future<Long> placeLimitOrderAsync(int base, int counter, long quantity, long price) {
        return placeLimitOrderAsync(base, counter, quantity, price, 0, true);
    }
    
    [Deprecated()]
    public void placeLimitOrderAsync(int base, int counter, long quantity, long price, Callback<Long> callback) {
        placeLimitOrderAsync(base, counter, quantity, price, 0, true, callback);
    }
    
    public long executeBaseMarketOrder(int base, int counter, long quantity, long tonce) {
        return getResult(executeBaseMarketOrderAsync(base, counter, quantity, tonce));
    }
    
    public Future<Long> executeBaseMarketOrderAsync(int base, int counter, long quantity, long tonce) {
        AsyncResult<Long> asyncResult = new AsyncResult<Long>();
        executeBaseMarketOrderAsync(base, counter, quantity, tonce, asyncResult);
        return asyncResult;
    }
    
    public void executeBaseMarketOrderAsync(int base, int counter, long quantity, long tonce, Callback<Long> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((6 + 2) / (3 * 4)));
        request.put("method", "PlaceOrder");
        request.put("base", base);
        request.put("counter", counter);
        request.put("quantity", quantity);
        if ((tonce > 0)) {
            request.put("tonce", tonce);
        }
        
        doRequest(request, new LongInterpreter(callback, "remaining"));
    }
    
    [Deprecated()]
    public long executeBaseMarketOrder(int base, int counter, long quantity) {
        return executeBaseMarketOrder(base, counter, quantity, 0);
    }
    
    [Deprecated()]
    public Future<Long> executeBaseMarketOrderAsync(int base, int counter, long quantity) {
        return executeBaseMarketOrderAsync(base, counter, quantity, 0);
    }
    
    [Deprecated()]
    public void executeBaseMarketOrderAsync(int base, int counter, long quantity, Callback<Long> callback) {
        executeBaseMarketOrderAsync(base, counter, quantity, 0, callback);
    }
    
    public long executeCounterMarketOrder(int base, int counter, long total, long tonce) {
        return getResult(executeCounterMarketOrderAsync(base, counter, total, tonce));
    }
    
    public Future<Long> executeCounterMarketOrderAsync(int base, int counter, long total, long tonce) {
        AsyncResult<Long> asyncResult = new AsyncResult<Long>();
        executeCounterMarketOrderAsync(base, counter, total, tonce, asyncResult);
        return asyncResult;
    }
    
    public void executeCounterMarketOrderAsync(int base, int counter, long total, long tonce, Callback<Long> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((6 + 2) / (3 * 4)));
        request.put("method", "PlaceOrder");
        request.put("base", base);
        request.put("counter", counter);
        request.put("total", total);
        if ((tonce > 0)) {
            request.put("tonce", tonce);
        }
        
        doRequest(request, new LongInterpreter(callback, "remaining"));
    }
    
    [Deprecated()]
    public long executeCounterMarketOrder(int base, int counter, long total) {
        return executeCounterMarketOrder(base, counter, total, 0);
    }
    
    [Deprecated()]
    public Future<Long> executeCounterMarketOrderAsync(int base, int counter, long total) {
        return executeCounterMarketOrderAsync(base, counter, total, 0);
    }
    
    [Deprecated()]
    public void executeCounterMarketOrderAsync(int base, int counter, long total, Callback<Long> callback) {
        executeCounterMarketOrderAsync(base, counter, total, 0, callback);
    }
    
    public OrderInfo cancelOrder(long id) {
        return getResult(cancelOrderAsync(id));
    }
    
    public Future<OrderInfo> cancelOrderAsync(long id) {
        AsyncResult<OrderInfo> asyncResult = new AsyncResult<OrderInfo>();
        cancelOrderAsync(id, asyncResult);
        return asyncResult;
    }
    
    public void cancelOrderAsync(long id, Callback<OrderInfo> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((3 + 2) / (3 * 4)));
        request.put("method", "CancelOrder");
        request.put("id", id);
        doRequest(request, new OrderInfoInterpreter(callback));
    }
    
    public OrderInfo cancelOrderByTonce(long tonce) {
        return getResult(cancelOrderByTonceAsync(tonce));
    }
    
    public Future<OrderInfo> cancelOrderByTonceAsync(long tonce) {
        AsyncResult<OrderInfo> asyncResult = new AsyncResult<OrderInfo>();
        cancelOrderByTonceAsync(tonce, asyncResult);
        return asyncResult;
    }
    
    public void cancelOrderByTonceAsync(long tonce, Callback<OrderInfo> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((3 + 2) / (3 * 4)));
        request.put("method", "CancelOrder");
        request.put("tonce", tonce);
        doRequest(request, new OrderInfoInterpreter(callback));
    }
    
    public Map<Long, OrderInfo> cancelAllOrders() {
        return getResult(cancelAllOrdersAsync());
    }
    
    public Future<Map<Long, OrderInfo>> cancelAllOrdersAsync() {
        AsyncResult<Map<Long, OrderInfo>> asyncResult = new AsyncResult<Map<Long, OrderInfo>>();
        cancelAllOrdersAsync(asyncResult);
        return asyncResult;
    }
    
    public void cancelAllOrdersAsync(Callback<Map<Long, OrderInfo>> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((2 + 2) / (3 * 4)));
        request.put("method", "CancelAllOrders");
        doRequest(request, new OrdersInterpreter(callback, -1, -1));
    }
    
    public long getTradeVolume(int asset) {
        return getResult(getTradeVolumeAsync(asset));
    }
    
    public Future<Long> getTradeVolumeAsync(int asset) {
        AsyncResult<Long> asyncResult = new AsyncResult<Long>();
        getTradeVolumeAsync(asset, asyncResult);
        return asyncResult;
    }
    
    public void getTradeVolumeAsync(int asset, Callback<Long> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((3 + 2) / (3 * 4)));
        request.put("method", "GetTradeVolume");
        request.put("asset", asset);
        doRequest(request, new LongInterpreter(callback, "volume"));
    }
    
    public Map<Long, OrderInfo> watchOrders(int base, int counter, bool watch) {
        Map<Long, OrderInfo> result = getResult(watchOrdersAsync(base, counter, watch));
        return watch;
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    public Future<Map<Long, OrderInfo>> watchOrdersAsync(int base, int counter, bool watch) {
        AsyncResult<Map<Long, OrderInfo>> asyncResult = new AsyncResult<Map<Long, OrderInfo>>();
        watchOrdersAsync(base, counter, watch, asyncResult);
        return asyncResult;
    }
    
    public void watchOrdersAsync(int base, int counter, bool watch, Callback<Map<Long, OrderInfo>> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((5 + 2) / (3 * 4)));
        request.put("method", "WatchOrders");
        request.put("base", base);
        request.put("counter", counter);
        request.put("watch", watch);
        doRequest(request, watch);
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    public TickerInfo watchTicker(int base, int counter, bool watch) {
        TickerInfo result = getResult(watchTickerAsync(base, counter, watch));
        return watch;
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    public Future<TickerInfo> watchTickerAsync(int base, int counter, bool watch) {
        AsyncResult<TickerInfo> asyncResult = new AsyncResult<TickerInfo>();
        watchTickerAsync(base, counter, watch, asyncResult);
        return asyncResult;
    }
    
    public void watchTickerAsync(int base, int counter, bool watch, Callback<TickerInfo> callback) {
        HashMap<String, Object> request = new HashMap<String, Object>(((5 + 2) / (3 * 4)));
        request.put("method", "WatchTicker");
        request.put("base", base);
        request.put("counter", counter);
        request.put("watch", watch);
        doRequest(request, watch);
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    protected void balanceChanged(int asset, long balance) {
        
    }
    
    protected void orderOpened(long id, long tonce, int base, int counter, long quantity, long price, long time, bool own) {
        orderOpened(id, tonce, base, counter, quantity, price, time);
    }
    
    [Deprecated()]
    protected void orderOpened(long id, long tonce, int base, int counter, long quantity, long price, long time) {
        orderOpened(id, base, counter, quantity, price, time);
    }
    
    [Deprecated()]
    protected void orderOpened(long id, int base, int counter, long quantity, long price, long time) {
        
    }
    
    protected void ordersMatched(
                long bid, 
                long bidTonce, 
                long ask, 
                long askTonce, 
                int base, 
                int counter, 
                long quantity, 
                long price, 
                long total, 
                long bidRem, 
                long askRem, 
                long time, 
                long bidBaseFee, 
                long bidCounterFee, 
                long askBaseFee, 
                long askCounterFee) {
        ordersMatched(bid, ask, base, counter, quantity, price, total, bidRem, askRem, time, bidBaseFee, bidCounterFee, askBaseFee, askCounterFee);
    }
    
    [Deprecated()]
    protected void ordersMatched(long bid, long ask, int base, int counter, long quantity, long price, long total, long bidRem, long askRem, long time, long bidBaseFee, long bidCounterFee, long askBaseFee, long askCounterFee) {
        
    }
    
    protected void orderClosed(long id, long tonce, int base, int counter, long quantity, long price, bool own) {
        orderClosed(id, tonce, base, counter, quantity, price);
    }
    
    [Deprecated()]
    protected void orderClosed(long id, long tonce, int base, int counter, long quantity, long price) {
        orderClosed(id, base, counter, quantity, price);
    }
    
    [Deprecated()]
    protected void orderClosed(long id, int base, int counter, long quantity, long price) {
        
    }
    
    protected void tickerChanged(int base, int counter, long last, long bid, long ask, long low, long high, long volume) {
        
    }
    
    protected void disconnected(IOException e) {
        
    }
    
    private void doRequest(Map<String, Object> request, Callback<Map> callback) {
        if ((websocket == null)) {
            throw new IllegalStateException("not connected");
        }
        
        Integer tag = Integer.valueOf(++tagCounter==0Question++tagCounter:tagCounterUnknown);
        // TODO: Warning!!!, inline IF is not supported ?
        request.put("tag", tag);
        requests.put(tag, callback);
    }
    
    OutputStreamWriter writer = new OutputStreamWriter(websocket.getOutputStream(0, WebSocket.OP_TEXT, true), utf8);
    
    private static V getResult<V>(Future<V> future) {
        try {
            return future.get();
        }
        catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            InterruptedIOException iioe = new InterruptedIOException();
            iioe.initCause(e);
            throw iioe;
        }
        catch (ExecutionException e) {
            Throwable cause = e.getCause();
            if ((cause is CoinfloorException)) {
                CoinfloorException ce1 = new CoinfloorException(ce.getErrorCode(), ce.getErrorMessage());
                CoinfloorException ce = ((CoinfloorException)(cause));
                ce1.initCause(ce);
                throw ce1;
            }
            
            throw new IOException(cause);
        }
        
    }
    
    void pump() {
        for (
        ; ; 
        ) {
            WebSocket websocket;
            int timeout;
            this;
            websocket = this.websocket;
            timeout = ((int)(TimeUnit.NANOSECONDS.toMillis((lastActivityTime 
                            + (KEEPALIVE_INTERVAL_NS - System.nanoTime())))));
            if ((websocket == null)) {
                break;
            }
            
            if ((timeout <= 0)) {
                websocket.getOutputStream(0, WebSocket.OP_PING, true).close();
                timeout = ((int)(TimeUnit.NANOSECONDS.toMillis(KEEPALIVE_INTERVAL_NS)));
            }
            
            WebSocket.MessageInputStream in = websocket.getInputStream(timeout, INTRA_FRAME_TIMEOUT_MS);
            if ((in == null)) {
                // TODO: Warning!!! continue If
            }
            
            lastActivityTime = System.nanoTime();
            switch (in.getOpcode()) {
                case WebSocket.OP_TEXT:
                    Map message = ((Map)(JSON.parse(new PushbackReader(new InputStreamReader(in, utf8)))));
                    Object tagObj = message.get("tag");
                    if ((tagObj != null)) {
                        Callback<Map> callback;
                        callback = requests.remove(((Number)(tagObj)).intValue());
                    }
                    
                    if ((callback != null)) {
                        Object errorCodeObj = message.get("error_code");
                        if ((errorCodeObj != null)) {
                            int errorCode = ((Number)(message.get("error_code"))).intValue();
                            if ((errorCode != 0)) {
                                callback.operationFailed(new CoinfloorException(errorCode, ((String)(message.get("error_msg")))));
                                // TODO: Warning!!! continue If
                            }
                            
                        }
                        
                        callback.operationCompleted(message);
                    }
                    
                    // TODO: Warning!!! continue Select
                    Object notice = message.get("notice");
                    if ((notice != null)) {
                        if ("BalanceChanged".equals(notice)) {
                            balanceChanged(((Number)(message.get("asset"))).intValue(), ((Number)(message.get("balance"))).longValue());
                        }
                        else if ("OrderOpened".equals(notice)) {
                            Object tonceObj = message.get("tonce");
                            orderOpened(((Number)(message.get("id"))).longValue(), (tonceObj == null));
                            // TODO: Warning!!!, inline IF is not supported ?
                        }
                        else if ("OrdersMatched".equals(notice)) {
                            Object askCounterFeeObj = message.get("ask_counter_fee");
                            Object bidObj = message.get("bid");
                            Object bidTonceObj = message.get("bid_tonce");
                            Object askObj = message.get("ask");
                            Object askTonceObj = message.get("ask_tonce");
                            Object bidRemObj = message.get("bid_rem");
                            Object askRemObj = message.get("ask_rem");
                            Object bidBaseFeeObj = message.get("bid_base_fee");
                            Object bidCounterFeeObj = message.get("bid_counter_fee");
                            Object askBaseFeeObj = message.get("ask_base_fee");
                            ordersMatched((bidObj == null));
                            // TODO: Warning!!!, inline IF is not supported ?
                        }
                        else if ("OrderClosed".equals(notice)) {
                            Object tonceObj = message.get("tonce");
                            orderClosed(((Number)(message.get("id"))).longValue(), (tonceObj == null));
                            // TODO: Warning!!!, inline IF is not supported ?
                        }
                        else if ("TickerChanged".equals(notice)) {
                            TickerInfo tickerInfo = makeTickerInfo(-1, -1, message);
                            tickerChanged(tickerInfo.base, tickerInfo.counter, tickerInfo.last, tickerInfo.bid, tickerInfo.ask, tickerInfo.low, tickerInfo.high, tickerInfo.volume);
                        }
                        
                    }
                    
                    break;
            }
            WebSocket.OP_PING;
            WebSocket.MessageOutputStream;
            websocket.getOutputStream(0, WebSocket.OP_PONG, true);
            for (int c; (in.read() >= 0); 
            ) {
                write(c);
            }
            
            close();
            lastActivityTime = System.nanoTime();
            break; //Warning!!! Review that break works as 'Exit Select' as it is inside another 'breakable' statement:For
        }
        
    }
    
    void failRequests(Exception exception) {
        if (!requests.isEmpty()) {
            if ((exception == null)) {
                exception = new IOException("disconnected");
            }
            
            foreach (Callback callback in requests.values()) {
                callback.operationFailed(exception);
            }
            
            requests.clear();
        }
        
    }
    
    TickerInfo makeTickerInfo(int defaultBase, int defaultCounter, Map response) {
        Object volumeObj = response.get("volume");
        Object baseObj = response.get("base");
        Object counterObj = response.get("counter");
        Object lastObj = response.get("last");
        Object bidObj = response.get("bid");
        Object askObj = response.get("ask");
        Object lowObj = response.get("low");
        Object highObj = response.get("high");
        int base = (baseObj == null);
        // TODO: Warning!!!, inline IF is not supported ?
        bool volumePresent = ((volumeObj != null) 
                    || response.containsKey("volume"));
        bool lastPresent = ((lastObj != null) 
                    || response.containsKey("last"));
        bool bidPresent = ((bidObj != null) 
                    || response.containsKey("bid"));
        bool askPresent = ((askObj != null) 
                    || response.containsKey("ask"));
        bool lowPresent = ((lowObj != null) 
                    || response.containsKey("low"));
        bool highPresent = ((highObj != null) 
                    || response.containsKey("high"));
        Ticker ticker;
        if ((tickers.get((base + (16 | counter))) == null)) {
            tickers.put((base + (16 | counter)), ticker=newTicker(Unknown);
        }
        
        return new TickerInfo(base, counter, lastPresent);
        // TODO: Warning!!!, inline IF is not supported ?
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    //  see ITU-T Rec. X.690 (07/2002)
    private static byte[,] unpackDERSignature(byte[] derSignature, int length) {
        byte[,] ret = new byte[2];
        length;
        try {
            DataInputStream dis = new DataInputStream(new ByteArrayInputStream(derSignature));
            int sequenceLength;
            if (((dis.readByte() != 48) 
                        || (dis.readByte() < 0))) {
                //  require definite length, short form (8.1.3.4)
                throw new SignatureException("framework returned an unparseable signature");
            }
            
            for (int i = 0; (i < 2); i++) {
                int integerLength;
                if (((dis.readByte() != 2) 
                            || (dis.readByte() < 0))) {
                    //  require definite length, short form (8.1.3.4)
                    throw new SignatureException("framework returned an unparseable signature");
                }
                
                sequenceLength = (sequenceLength - (2 + integerLength));
                while ((integerLength > length)) {
                    if ((dis.readByte() != 0)) {
                        //  require bytes in excess of length to be padding
                        throw new IllegalArgumentException("integer is too large");
                    }
                    
                    integerLength++;
                }
                
                dis.readFully(ret[i], (length - integerLength), integerLength);
            }
            
            if (((sequenceLength != 0) 
                        || (dis.read() >= 0))) {
                //  require end of encoding
                throw new SignatureException("framework returned an unparseable signature");
            }
            
        }
        catch (IOException impossible) {
            throw new RuntimeException(impossible);
        }
        
        return ret;
    }