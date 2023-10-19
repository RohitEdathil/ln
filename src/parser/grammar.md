Basic grammar

```
expression     → literal
               | unary
               | binary
               | grouping ;

literal        → NUMBER | STRING | "true" | "false" | "nil" ;
grouping       → "(" expression ")" ;
unary          → ( "-" | "!" ) expression ;
binary         → expression operator expression ;
operator       → "==" | "!=" | "<" | "<=" | ">" | ">="
               | "+"  | "-"  | "*" | "/" ;
```

Unambiguous grammar (lowest precedence to highest precedence)

```
expression     → equality ;
equality       → comparison ( ( "!=" | "==" )             comparison )* ;
comparison     → term       ( ( ">" | ">=" | "<" | "<=" ) term       )* ;
term           → factor     ( ( "-" | "+" )               factor     )* ;
factor         → unary      ( ( "/" | "*" )               unary      )* ;
unary          → ( "!" | "-" ) unary
               | primary ;
primary        → NUMBER | STRING | "true" | "false" | "nil"
               | "(" expression ")" ;
```
