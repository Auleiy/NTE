int cycle = int.Parse(Console.ReadLine());

Random r = new();
Console.Write('[');
for (int i = 0; i < cycle; i++)
{
    if (i % 16 == 0 && i != 0)
        Console.Write("\n ");
    Console.Write($"{r.Next(1025):0000}, ");
}
Console.CursorLeft -= 2;
Console.Write(']');