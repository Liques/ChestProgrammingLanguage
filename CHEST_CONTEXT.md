buildingDecl   : 'building' IDENT block ;
block          : '{' stmt* '}' ;
stmt           : officeDecl
               | employeeDecl
               | varDecl ';'
               | showStmt ';'
               | decideStmt
               ;
officeDecl     : 'office' IDENT block ;
employeeDecl   : 'employee' IDENT paramList? block ;
paramList      : '(' (IDENT (',' IDENT)*)? ')' ;
varDecl        : 'chest' IDENT ('=' expr)? ;
showStmt       : 'show' expr ;
decideStmt     : 'decide' expr block ('else' block)? ;
expr           : literal
               | IDENT
               | expr binop expr
               | '(' expr ')'
               ;
literal        : NUMBER | STRING | 'true' | 'false' ;
binop          : '+' | '-' | '*' | '/' | '<' | '>' | '==' | '!=' | '<=' | '>=' ;
```
il.Emit(OpCodes.Call, showMethodInfo);
// This file has been replaced by CHEST_GUIDE.md in Chest.Compiler. See CHEST_GUIDE.md for the latest documentation.
