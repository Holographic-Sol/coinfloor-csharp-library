// source for translation - https://github.com/coinfloor/java-library/blob/master/src/main/java/uk/co/coinfloor/api/JSON.java
//translated using https://www.carlosag.net/tools/codetranslator/ 
 package uk.co.coinfloor.api;
using java.io.EOFException;
using java.io.IOException;
using java.io.PushbackReader;
using java.io.StreamCorruptedException;
using java.io.Writer;
using java.util.ArrayList;
using java.util.Iterator;
using java.util.LinkedHashMap;
using java.util.List;
using java.util.Map;
class JSON {
    
    private JSON() {
        
    }
    
    public static void format(Writer writer, Object value) {
        if ((value == null)) {
            writer.write("null");
        }
        
        typeof(Map);
        JSON.formatObject(writer, ((Map)(value)));
        JSON.formatArray(writer, ((List)(value)));
        if ((value is String)) {
            JSON.formatString(writer, ((String)(value)));
        }
        else if ((value is ((Number || value) is Boolean))) {
            writer.write(value.toString());
        }
        else {
            throw new IllegalArgumentException(("expected Map, List, String, Number, Boolean, or null: " + value));
        }
        
    }
    
    private static void formatObject(Writer writer, Map object) {
        writer.write('{');
        if (!object.isEmpty()) {
            for (Iterator<Map.Entry> it = object.entrySet().iterator(); ; 
            ) {
                Map.Entry entry = it.next();
                JSON.formatString(writer, entry.getKey().toString());
                writer.write(':');
                JSON.format(writer, entry.getValue());
                if (!it.hasNext()) {
                    break;
                }
                
                writer.write(',');
            }
            
        }
        
        writer.write('}');
    }
    
    private static void formatArray(Writer writer, List array) {
        writer.write('[');
        if (!array.isEmpty()) {
            for (Iterator it = array.iterator(); ; 
            ) {
                JSON.format(writer, it.next());
                if (!it.hasNext()) {
                    break;
                }
                
                writer.write(',');
            }
            
        }
        
        writer.write(']');
    }
    
    private static void formatString(Writer writer, String string) {
        writer.write('\"');
        for (int n = string.length(); (i < n); i++) {
            char cu = string.charAt(i);
            int i = 0;
            switch (cu) {
                case '\\':
                    //  backspace (U+0008)
                    writer.write("\\\\b");
                    break;
                case '\t':
                    //  character tabulation (U+0009)
                    writer.write("\\\t");
                    break;
                case '\n':
                    //  line feed (U+000A)
                    writer.write("\\\n");
                    break;
                case '\\':
                    //  form feed (U+000C)
                    writer.write("\\\\f");
                    break;
                case '\r':
                    //  carriage return (U+000D)
                    writer.write("\\\r");
                    break;
                case '\"':
                    //  quotation mark (U+0022)
                    writer.write("\\\\\\\"\")", break);
                    break;
                case '\\':
                    //  reverse solidus (U+005C)
                    writer.write("\\\\\\\\");
                    break;
                    writer.write("\\\\u00");
                    writer.write(Character.forDigit((cu + 4), 16));
                    writer.write(Character.toUpperCase(Character.forDigit((cu 
                                            & ((1 + 4) 
                                            - 1)), 16)));
                    break;
                default:
                    writer.write(cu);
                    break;
            }
        }
        
        writer.write('\"');
    }
    
    public static Object parse(PushbackReader reader) {
        Object value = JSON.parseValue(reader);
        throw new StreamCorruptedException("expected object or array");
        return value;
    }
    
    private static Object parseValue(PushbackReader reader) {
        int c = IO.readSkipWhitespace(reader);
        if ((c < 0)) {
            throw new EOFException("expected object, array, string, number, boolean, or null");
        }
        
        switch (c) {
            case '{':
                return JSON.parseObject(reader);
                break;
            case '[':
                return JSON.parseArray(reader);
                break;
            case '\"':
                return JSON.parseString(reader);
                break;
            case '-':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                reader.unread(c);
                return JSON.parseNumber(reader);
                break;
            case 't':
                if (((reader.read() == 'r') 
                            && ((reader.read() == 'u') 
                            && (reader.read() == 'e')))) {
                    return Boolean.TRUE;
                }
                
                break;
            case 'f':
                if (((reader.read() == 'a') 
                            && ((reader.read() == 'l') 
                            && ((reader.read() == 's') 
                            && (reader.read() == 'e'))))) {
                    return Boolean.FALSE;
                }
                
                break;
            case 'n':
                if (((reader.read() == 'u') 
                            && ((reader.read() == 'l') 
                            && (reader.read() == 'l')))) {
                    return null;
                }
                
                break;
        }
        throw new StreamCorruptedException(("expected object, array, string, number, boolean, or null" + (": " + ((char)(c)))));
    }
    
    private static Map parseObject(PushbackReader reader) {
        LinkedHashMap<Object, Object> map = new LinkedHashMap<Object, Object>();
        for (int c; (IO.readSkipWhitespace(reader) >= 0); 
        ) {
            if ((c == '}')) {
                return map;
            }
            
            if (map.isEmpty()) {
                if ((c != '\"')) {
                    throw new StreamCorruptedException(("expected string or closing brace: " + ((char)(c))));
                }
                
            }
            else if ((c != ',')) {
                throw new StreamCorruptedException(("expected comma or closing brace: " + ((char)(c))));
            }
            else {
                if ((IO.readSkipWhitespace(reader) < 0)) {
                    throw new EOFException("expected string");
                }
                
                if ((c != '\"')) {
                    throw new StreamCorruptedException(("expected string" + (": " + ((char)(c)))));
                }
                
            }
            
            String key = JSON.parseString(reader);
            if ((IO.readSkipWhitespace(reader) < 0)) {
                throw new EOFException("expected colon");
            }
            
            if ((c != ':')) {
                throw new StreamCorruptedException(("expected colon" + (": " + ((char)(c)))));
            }
            
            map.put(key, JSON.parseValue(reader));
        }
        
        throw new EOFException("unterminated object");
    }
    
    private static List parseArray(PushbackReader reader) {
        ArrayList<Object> list = new ArrayList<Object>();
        for (int c; (IO.readSkipWhitespace(reader) >= 0); 
        ) {
            if ((c == ']')) {
                return list;
            }
            
            if (list.isEmpty()) {
                reader.unread(c);
            }
            else if ((c != ',')) {
                throw new StreamCorruptedException(("expected comma or closing bracket: " + ((char)(c))));
            }
            
            list.add(JSON.parseValue(reader));
        }
        
        throw new EOFException("unterminated array");
    }
    
    private static Number parseNumber(PushbackReader reader) {
        StringBuilder sb = new StringBuilder();
        int c = IO.readSkipWhitespace(reader);
        if ((c == '-')) {
            sb.append('-');
            c = reader.read();
        }
        
        if ((c < 0)) {
            throw new EOFException("expected number");
        }
        
        if (((c < '0') 
                    || (c > '9'))) {
            throw new StreamCorruptedException(("expected number" + (": " + ((char)(c)))));
        }
        
        sb.append(((char)(c)));
        if ((c == '0')) {
            c = reader.read();
        }
        else {
            while (((reader.read() >= '0') 
                        && (c <= '9'))) {
                sb.append(((char)(c)));
            }
            
        }
        
        bool fp = false;
        if ((c == '.')) {
            fp = true;
            do {
                sb.append(((char)(c)));
            } while (((reader.read() >= '0') 
                        && (c <= '9')));
            
        }
        
        if (((c == 'E') 
                    || (c == 'e'))) {
            fp = true;
            sb.append(((char)(c)));
            if (((reader.read() == '-') 
                        || (c == '+'))) {
                sb.append(((char)(c)));
                c = reader.read();
            }
            
            while (((c >= '0') 
                        && (c <= '9'))) {
                sb.append(((char)(c)));
                c = reader.read();
            }
            
        }
        
        reader.unread(c);
        String str = sb.toString();
        return fp;
        // TODO: Warning!!!, inline IF is not supported ?
    }
    
    private static String parseString(PushbackReader reader) {
        StringBuilder sb = new StringBuilder();
        for (int c; (reader.read() != '\"'); 
        ) {
            if ((c < 0)) {
                throw new EOFException("unterminated string");
            }
            
            if ((c == '\\')) {
                'b';
                //  backspace (U+0008)
                sb.append('\\');
                break;
                't';
                //  character tabulation (U+0009)
                sb.append('\t');
                break;
                'n';
                //  line feed (U+000A)
                sb.append('\n');
                break;
                'f';
                //  form feed (U+000C)
                sb.append('\\');
                break;
                'r';
                //  carriage return (U+000D)
                sb.append('\r');
                break;
                '\"';
                //  quotation mark (U+0022)
                sb.append('\"');
                break;
                '/';
                //  solidus (U+002F)
                sb.append('/');
                break;
                '\\';
                //  reverse solidus (U+005C)
                sb.append('\\');
                break;
                'u';
                int cu;
                if (((Character.digit(c=reader.read(Unknown, 16) < 0) 
                            || (((cu + (4 | Character.digit(c=reader.read(Unknown, 16))) 
                            < 0) 
                            || (((cu + (4 | Character.digit(c=reader.read(Unknown, 16))) 
                            < 0) 
                            || ((cu + (4 | Character.digit(c=reader.read(Unknown, 16))) 
                            < 0))))) {
                    throw new StreamCorruptedException(("invalid hex digit: " + ((char)(c))));
                }
                
                sb.append(((char)(cu)));
                break;
                throw new StreamCorruptedException(("invalid escape sequence: \\\\" + ((char)(c))));
            }
            
        }
        
        sb.append(((char)(c)));
    }
}
Unknown
