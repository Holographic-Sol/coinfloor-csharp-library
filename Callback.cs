// source for translation - https://github.com/coinfloor/java-library/blob/master/src/main/java/uk/co/coinfloor/api/Callback.java
//translated using https://www.carlosag.net/tools/codetranslator/
package uk.co.coinfloor.api;
public interface Callback<V> {
    
    void operationCompleted(V result);
    
    void operationFailed(Exception exception);
}