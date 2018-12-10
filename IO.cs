 // source for translation https://github.com/coinfloor/java-library/blob/master/src/main/java/uk/co/coinfloor/api/IO.java
//translated using https://www.carlosag.net/tools/codetranslator/
 package uk.co.coinfloor.api;
using java.io.EOFException;
using java.io.IOException;
using java.io.Reader;
class IO {
    
    private IO() {
        
    }
    
    static int readSkipWhitespace(Reader reader) {
        int c;
        do {
            if ((reader.read() < 0)) {
                return -1;
            }
            
        } while (Character.isWhitespace(((char)(c))));
        
        return c;
    }
    
    static String readUntil(Reader reader, char stop) {
        return IO.copyUntil(reader, new StringBuilder(), stop).toString();
    }
    
    static StringBuilder copyUntil(Reader reader, StringBuilder sb, char stop) {
        for (int c; (reader.read() >= 0); 
        ) {
            if ((c == stop)) {
                return sb;
            }
            
            sb.append(((char)(c)));
        }
        
        throw new EOFException();
    }
    
    static void skipUntil(Reader reader, char stop) {
        for (int c; (reader.read() >= 0); 
        ) {
            if ((c == stop)) {
                return;
            }
            
        }
        
        throw new EOFException();
    }
    
    static String readRemaining(Reader reader) {
        return IO.copyRemaining(reader, new StringBuilder()).toString();
    }
    
    static StringBuilder copyRemaining(Reader reader, StringBuilder sb) {
        for (int c; (reader.read() >= 0); 
        ) {
            sb.append(((char)(c)));
        }
        
        return sb;
    }
}
