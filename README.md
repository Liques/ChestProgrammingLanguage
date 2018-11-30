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

## Is it complete?
I'm sorry... For a while, it's a concept yet, I'm still building it.

## Where can I run this language?
We have a prototype compiler in this repository where is possible to compile the language, but it was built only to test the 
language concept, a compiled version of this compiler is not available yet.

## License

The Chest Language syntax, any Chest Language specification and the underlying source code used to format and display that content is licensed under the [MIT license](LICENSE.md).
