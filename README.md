
# Chest

Chest is a programming language designed to be easy and to be learned quickly.

  

To become "easy", the Chest language try to have its syntax closer to our real world using familiar concepts.

  

## Why a new programming language?

  

The lack of developers worldwide show us that learn a programming language is not easy for anyone. In Brazil, most of IT's

university students [give up their course](https://www.cbsi.net.br/2018/02/curso-de-computacao-e-um-dos-que-mais.html) before

the end. Most of these languages also are available only in English, what makes it most difficult for who

don't know this language and must to learn another language at the same time.

  

The Chest language was designed to be learned very quickly even for people who never developed before. Instead of make

the new developers understand the main developing concepts, the Chest language try to adapt itself to concepts that are

common in our real world.

  

## How easy is it?

  

For example: Let's say that you want to build a business building. How a business is divided? Well, a business is divided into

offices. What we have in offices? Employees and some furniture. The furniture by itself does not do anything beyond to keep some

storage. But the employees are supposed to do something in the office, they always have a function.

  

Let's say that there is an office, in some building. You are inside this office. This office has a very old chest. But

there is something very weird in this chest, there is a phrase in this chest saying "Hello World!" and it is showing up to you!

Done, you already know the basic concepts of Chest language!

  

Looks now in code:

  

```

building MyBusinessBuilding

office Reception

employee Reception

chest helloWorldChest

"Hello World"

show helloWorldChest

```

  

Another example a little bit more complex using .NET libraries.

  

```

building MyBusinessBuilding

office Lobby

employee NameChecker

chest name

go Console

poke ReadLine

chest rightName

""John""

go Console

poke WriteLine

""The name is {0}""

name

decide

name == rightName

go Console

poke WriteLine

""The name is John""

  

name != rightName

go Console

poke WriteLine

""The name is NOT John""

```

## Programming Language Reference ##

  

### Spacing ###

  

The spacing standard is like in Python, it is tabular. This means that the code blocks inside a building, an office, an employee, a chest, or a decide command must be indented with a tab or four spaces.

  

### Keywords ###

  



#### building ####
The base of the language, like a namespace or a package in other languages. All parts of the language must be inside a building.

```
// C#
namespace MyBusinessBuilding {
    ...
}

// Java
package MyBusinessBuilding;
...

// JavaScript
var MyBusinessBuilding = {
    ...
};
```

##### office ####
An object type, like a class or an object in other languages. Instead of being an object, it is like a place inside the building.

```
// C#
class Reception {
    ...
}

// Java
class Reception {
    ...
}

// JavaScript
var Reception = {
    ...
};
```

#### employee ####
A method, like a void method or a function in other languages. It is supposed to do something in the office.

```
// C#
void Reception() {
    ...
}

// Java
void Reception() {
    ...
}

// JavaScript
function Reception() {
    ...
}
```

#### chest ####
A variable, like a string or a number in other languages. It can contain text or numbers, like a double.

```
// C#
string helloWorldChest = "Hello World";
or
double helloWorldChest = 1.0;

// Java
String helloWorldChest = "Hello World";
or
double helloWorldChest = 1.0;

// JavaScript
var helloWorldChest = "Hello World";
or
var helloWorldChest = 1.0;
```

#### go ####
A command to go to like a physical place, like accessing an object or a property in other programming languages. It is used to go to an office or a chest.

```
// C#
Console.WriteLine(...);
or
helloWorldChest.Length;

// Java
System.out.println(...);
or
helloWorldChest.length();

// JavaScript
console.log(...);
or
helloWorldChest.length;
```

#### poke ####
A command to poke something, like using a dot or calling a method in other languages. It is used to poke an employee or a chest inside an office.

```
// C#
Console.ReadLine();
or
helloWorldChest.ToUpper();

// Java
scanner.nextLine();
or
helloWorldChest.toUpperCase();

// JavaScript
prompt();
or
helloWorldChest.toUpperCase();
```

#### show ####
A command to show something, like printing to the console or alerting in other languages. It is used to show the content of a chest.

```
// C#
Console.WriteLine(helloWorldChest);

// Java
System.out.println(helloWorldChest);

// JavaScript
console.log(helloWorldChest);
or
alert(helloWorldChest);
```

#### decide ####
A command to decide something, like an if condition or a ternary operator in other languages. It is used to compare chests and execute different code blocks.

```
// C#
if (helloWorldChest == "1") {
    ...
} else if (helloWorldChest != "1") {
    ...
}

// Java
if (helloWorldChest.equals("1")) {
    ...
} else if (!helloWorldChest.equals("1")) {
    ...
}

// JavaScript
if (helloWorldChest == "1") {
    ...
} else if (helloWorldChest != "1") {
    ...
}
or 
(helloWorldChest == "1") ? ... : (helloWorldChest != "1") ? ... : ...;
```


  
  
  

## Is it complete?

It is still only a concept. There was a older attepmt to build a compiler, but this project was discontinued.