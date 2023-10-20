using Xunit;

namespace Ilmhub.Judge.Sdk.Compiler.Tests.Integration;

public class LanguageTheoryData : TheoryData<string, int, string> {
    public LanguageTheoryData() {
        Add("C", 1,
            @"
                #include <stdio.h>
                int main() {
                    int num1, num2;
                    scanf(""%d %d"", &num1, &num2);
                    printf(""Sum: %d\n"", num1 + num2);
                    return 0;
                }");

        Add("C++", 2,
            @"
                #include <iostream>
                using namespace std;
                int main() {
                    int num1, num2;
                    cin >> num1 >> num2;
                    cout << ""Sum: "" << num1 + num2 << endl;
                    return 0;
                }");

        Add("C#", 3,
            @"
                using System;
                class Program {
                    static void Main() {
                        int num1, num2;
                        string input = Console.ReadLine();
                        string[] numbers = input.Split();
                        num1 = int.Parse(numbers[0]);
                        num2 = int.Parse(numbers[1]);
                        Console.WriteLine(""Sum: "" + (num1 + num2));
                    }
                }");

        Add("Python3", 4,
            @"
num1, num2 = map(int, input().split())
print('Sum:', num1 + num2)");

        Add("Go", 5,
            @"
                package main

                import (
                    ""fmt""
                )

                func main() {
                    var num1, num2 int
                    fmt.Scan(&num1, &num2)
                    fmt.Println(""Sum:"", num1 + num2)
                }");

        // Add("Java", 6,
        //     @"
        //         import java.util.Scanner;

        //         public class Main {
        //             public static void main(String[] args) {
        //                 Scanner scanner = new Scanner(System.in);
        //                 int num1 = scanner.nextInt();
        //                 int num2 = scanner.nextInt();
        //                 System.out.println(""Sum: "" + (num1 + num2));
        //             }
        //         }",
        //     @"
        //         import java.util.Scanner;

        //         public class Main {
        //             public static void main(String[] args) {
        //                 Scanner scanner = new Scanner(System.in);
        //                 int num1 = scanner.nextInt();
        //                 int num2 = scanner.nextInt()            // missing semi colon
        //                 System.out.println(""Sum: "" + (num1 + num2));
        //             }
        //         }");
    }
}