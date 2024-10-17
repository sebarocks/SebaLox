using System.Text;
using System.Xml.Linq;

namespace Tool
{
    public class GenerateAST
    {
        public static void Generate(string[] args)
        {
            if(args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(64);
            }
            String outputDir = args[0];
            defineAST(outputDir, "Expr", new List<string>
            {
                "Binary     : Expr left, Token operator, Expr right",
                "Grouping   : Expr expression",
                "Literal    : Object value",
                "Unary      : Token operator, Expr right"
            });
        }

        private static void defineAST(string outputDir, string baseName, List<string> types)
        {
            string path = $"{outputDir}/{baseName}.cs";

            using (StreamWriter streamWriter = new StreamWriter(path, false , Encoding.UTF8)) 
            {
                var writer = new IndentedWriter(streamWriter);
                writer.WriteLine("namespace LoxNet;");
                writer.WriteLine();

                writer.WriteLine($"public abstract class {baseName}");
                writer.WriteLine("{");
                writer.IncreaseIndent();

                defineVisitor(writer, baseName, types);

                foreach (string type in types)
                {
                    string className = type.Split(":")[0].Trim();
                    string fields = type.Split(":")[1].Trim();
                    defineType(writer, baseName, className, fields);
                }

                writer.WriteLine();
                writer.WriteLine("public abstract R Accept<R>(Visitor<R> visitor);");

                writer.DecreaseIndent();
                writer.WriteLine("}");
            }
        }

        private static void defineVisitor(IndentedWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine("public interface Visitor<R>");
            writer.WriteLine("{");
            writer.IncreaseIndent();

            foreach (string type in types)
            {
                string typename = type.Split(":")[0].Trim();
                writer.WriteLine($"R Visit{typename}{baseName}({typename} {baseName.ToLower()});");
            }

            writer.DecreaseIndent();
            writer.WriteLine("}");
        }

        private static void defineType(IndentedWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine($"public class {className} : {baseName}");
            writer.WriteLine("{");
            writer.IncreaseIndent();

            writer.WriteLine($"public {className}({fieldList})");
            writer.WriteLine("{");
            writer.IncreaseIndent();

            string[] fields = fieldList.Split(", ");
            foreach (string field in fields)
            {
                string name = field.Split(" ")[1];
                string nameCapitalized = char.ToUpper(name[0]) + name.Substring(1);
                writer.WriteLine($"this.{nameCapitalized} = {name};");
            }

            writer.DecreaseIndent();
            writer.WriteLine("}");

            writer.WriteLine();

            writer.WriteLine("public override R Accept<R>(Visitor<R> visitor)");
            writer.WriteLine("{");
            writer.IncreaseIndent();
            writer.WriteLine("return visitor.Visit" + className + baseName + "(this);" );
            writer.DecreaseIndent();
            writer.WriteLine("}");

            writer.WriteLine();

            foreach (string field in fields)
            {
                string[] fieldInfo = field.Split(" ");
                writer.WriteLine($"public readonly {fieldInfo[0]} {Capitalize(fieldInfo[1])};");
            }

            writer.DecreaseIndent();
            writer.WriteLine(" }");
        }

        static string Capitalize(string capitalized) {
            return char.ToUpper(capitalized[0]) + capitalized.Substring(1);
        }
    }

    
}
