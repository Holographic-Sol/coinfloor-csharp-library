package uk.co.coinfloor.api;
// source for translation https://github.com/coinfloor/java-library/blob/master/src/main/java/uk/co/coinfloor/api/AsyncResult.java
//translated using https://www.carlosag.net/tools/codetranslator/
using java.util.concurrent.ExecutionException;
using java.util.concurrent.Future;
using java.util.concurrent.TimeUnit;
using java.util.concurrent.TimeoutException;
class AsyncResult<V> : Future<V>, Callback<V> {
    
    private int state;
    
    private Object result;
    
    [Override()]
    public bool cancel(bool mayInterruptIfRunning) {
        return false;
    }
    
    [Override()]
    public bool isCancelled() {
        return false;
    }
    
    [Override()]
    public bool isDone() {
        return (this.state != 0);
    }
    
    [Override()]
    [SuppressWarnings("unchecked")]
    public V get() {
        while ((this.state == 0)) {
            wait();
        }
        
        if ((this.state < 0)) {
            throw new ExecutionException(((Throwable)(this.result)));
        }
        
        return ((V)(this.result));
    }
    
    [Override()]
    [SuppressWarnings("unchecked")]
    public V get(long timeout, TimeUnit unit) {
        if ((this.state == 0)) {
            for (long deadline = (System.nanoTime() + unit.toNanos(timeout)); ; 
            ) {
                TimeUnit.NANOSECONDS.timedWait(this, timeout);
                if ((this.state != 0)) {
                    break;
                }
                
                if (((deadline - System.nanoTime()) 
                            <= 0)) {
                    throw new TimeoutException();
                }
                
            }
            
        }
        
        if ((this.state < 0)) {
            throw new ExecutionException(((Throwable)(this.result)));
        }
        
        return ((V)(this.result));
    }
    
    [Override()]
    public void operationCompleted(V result) {
        this.state = 1;
        this.result = this.result;
        notifyAll();
    }
    
    [Override()]
    public void operationFailed(Exception exception) {
        this.state = -1;
        this.result = exception;
        notifyAll();
    }
}