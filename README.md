# Experimental

## Purpose
The purpose of this repository is to experiment with functional aspects of c#.
There are a few rules that I want to stick by and see if it is possible to use them in a real world application.
The rules are:-
- Use C# records for data objects.
- Use only static classes with static functions.
- Reduce Interfaces
- Pass all dependencies into the function instead of objects.
- Remove the dependency on dependency injection framework.
- Be more LINQy

## Why these rules?
- I want to check if it makes the code easier to read and understand.
- I want to reduce the cognitive load in understanding what makes the application work. By focusing on functions instead of objects, I think my brain can focus only on the function in front of me rather than the object because everything that function needs will be passed into it. 
- Reduce Interfaces? I don't have anything against interfaces but they tend to explode in number especially in large applications. And usually, in a large number of cases, they are implemented by just one class. If I can eliminate the need for them, the better.
- Be more LINQy? Personally, I like how LINQ can help reduce the number of lines you write and the number of temporary variables you create in a function call. It also helps me think in such a way that I want to reduce my function to return in just one line or as they say "expressions" in the functional world. The Linq library is a fantastic library that helps in this goal.

## What next?
Once I am comfortable and happy with what I have done. I plan to use these rules on a real non-trivial application called OrchardCoreCMS to see how it translates and if it helps in code readability and understandability of the application.
