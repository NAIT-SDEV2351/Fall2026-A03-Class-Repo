namespace ConsoleApp1
{
    internal class Program
    {
        // A delegate is a type of variable that you can assign a method to.
        // When creating a delegate, you are defining the shape of the method that can
        // be assigned. In the case of "Notify", it is a void method that accepts a single string as
        // a parameter, so only methods with that exact same method definition can be assigned
        delegate void Notify(string str);
        static void Main(string[] args)
        {
            // Will work because SayHello matches the delegate
            Notify sendMessage = SayHello;

            // This will error because Add does not match
            // Notify addNumbers = Add;

            // This lambda method (a.k.a. anonymous method) matches the pattern defined in
            // the delegate ("void Method(string)"), so it will work.
            Notify lambdaMethod = (job) => 
            { 
                Console.WriteLine($"You are a {job}"); 
            };

            // use the variable the same way as you would a regular method call.
            lambdaMethod("Sheep Barber");
        }
        static public void Handler(object? sender, EventArgs args) { }
        // Fits the pattern "void Method(string)", so it can be assigned to the Notify delegate
        static void SayHello(string name)
        {
            Console.WriteLine($"Hello {name}!");
        }
        // Does not fit the pattern "void Method(string)", so it cannot be assigned to Notify
        static int Add(int a, int b)
        {
            return a + b;
        }
    }
}
