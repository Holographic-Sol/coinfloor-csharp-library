// source for translation - https://github.com/coinfloor/java-library/blob/master/src/main/java/uk/co/coinfloor/api/CoinfloorException.java
//translated using https://www.carlosag.net/tools/codetranslator/
package uk.co.coinfloor.api;
public class CoinfloorException : Exception {
    
    private static long serialVersionUID;
    
    private int errorCode;
    
    public CoinfloorException(int errorCode, String errorMessage) : 
            base(errorMessage) {
        base.(errorMessage);
        this.errorCode = this.errorCode;
    }
    
    public int getErrorCode() {
        return this.errorCode;
    }
    
    public String getErrorMessage() {
        return base.getMessage();
    }
    
    [Override()]
    public String getMessage() {
        String errorMessage = base.getMessage();
        return (errorMessage == null);
        // TODO: Warning!!!, inline IF is not supported ?
    }
}