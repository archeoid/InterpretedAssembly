# InterpretedAssembly
### A C# GUI editor for an simple interpreted assembly-like language.

The language itself is very simple, only having basic arithmetic and register manipulation. However there are more advanced features such as the indented logic system, outputting to a pixel grid and stack based return jumps.

---
### Info

The quality of the code for this project is subpar, i wrote it before getting any formal programming teaching back in 2018. It does howver function properly as far as i can see.

---
![Demo Gif](https://github.com/archeoid/InterpretedAssembly/blob/master/images/demo1.gif)

This program calculates the first few terms of the fibbonacci sequence. The delay between executed instructions can be set in the `More Options` menu, or you can select `Forward Until End` to execute the entire program at once. The language specification is also accessable in the options menu.

---
![GCD](https://github.com/archeoid/InterpretedAssembly/blob/master/images/gcd.PNG)

This program calculates the gcd of the input numbers, employing the return jump, `RJP`, to form a GCD function. How `RJP` works is that when it is provided with a label parameter, it will push the current position onto a stack then jump to the label. Then when calling `RJP` without a parameter, it will pop a position off the stack and jump to it. This allows for the creation of modular code (given the registers line up).

---
![LOGIC](https://github.com/archeoid/InterpretedAssembly/blob/master/images/logic.PNG)

This is a program written to test the logic system. How the logic works is also stack based, when a logic instruction is executed a `+` or `-` is pushed onto the stack, depending on the result. Then when IA encounters a line with `+-` prefixs, it will check if the prefix matches the logic stack and conditionally execute that line. If the prefix is shorter than the stack (including no prefix), the stack is shortened to match.

---
![CIRCLE](https://github.com/archeoid/InterpretedAssembly/blob/master/images/circle.PNG)

This is what the pixel grid looks like. This program is written to draw a circle, but there are no triginometric instructions so this is less than efficient code.
The pixel grid is controlled by 3 instructions, `PXL X Y V` (which sets a pixel black or white), `HSV X Y H S V` (which sets a pixels color from HSV space), and `RGB X Y R G B` (same as `HSV` but for RGB space).

---
![HSV](https://github.com/archeoid/InterpretedAssembly/blob/master/images/hsv.PNG)

A pixel grid example using `HSV`. The size of the grid can be set in `More Options`.

---
## Dependancies
* AvalonEdit (Syntax Highlighting)
* SlimDX (Pixel Grid)
* Fody (i cant remember)
