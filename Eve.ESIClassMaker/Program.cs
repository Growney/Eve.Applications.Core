using Eve.ESIClassMaker.CodeObjects;
using Eve.ESIClassMaker.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESIClassMaker
{
    public class Program
    {
        private static List<string> m_versions;
        public static async Task Main(string[] args)
        {
            Console.WriteLine("string1".GetHashCode() == "string1".GetHashCode());
            Console.WriteLine("Welcome to the ESI Class maker");
            string input;
            do
            {
                Console.WriteLine("Please input the version you wish to parse or ? for a list of avaliable versions or type q to quit:");
                input = Console.ReadLine();
                if (input == "?")
                {
                    await ShowVersions();
                }
                else
                {
                    await GetVersions();
                    if (VersionExists(input))
                    {
                        Console.WriteLine($"Getting Schema for version {input}:");
                        var schemaTask = Schema.ESISchema.GetSchemaAsync(input);
                        await schemaTask;

                        if (schemaTask.IsCompleted)
                        {
                            Schema.ESISchema schema = schemaTask.Result;
                            Dictionary<int, path> pathIDs = new Dictionary<int, path>();
                            int methodCount = 1;
                            foreach (string path in schema.paths.Keys)
                            {
                                //Console.WriteLine(path);
                                foreach(string method in schema.paths[path].Keys)
                                {
                                    path p = schema.paths[path][method];
                                    p.pathMethod = method;
                                    p.pathString = path;
                                    Console.WriteLine($"{methodCount:000} - {path} - {method}:{p.summary}");
                                    pathIDs.Add(methodCount, p);
                                    methodCount++;
                                }
                            }
                            do
                            {
                                Console.WriteLine("Please select a path number to generate a class for or type b to return to the main menu:");
                                input = Console.ReadLine();
                                if(input != "b")
                                {
                                    if (Int32.TryParse(input, out int selection))
                                    {
                                        if (pathIDs.ContainsKey(selection))
                                        {
                                            path selected = pathIDs[selection];
                                            
                                            List<ClassObject> classes = selected.GetSuccessClass();
                                            if(classes.Count > 0)
                                            {
                                                Console.WriteLine($"Creating {classes.Count} classes for path {selected.summary}");
                                                string directory = "Output";
                                                if (!System.IO.Directory.Exists(directory))
                                                {
                                                    System.IO.Directory.CreateDirectory(directory);
                                                }
                                                for (int i = 0; i < classes.Count; i++)
                                                {
                                                    Console.WriteLine($"Please select a new class name for class {classes[i].Name} or press enter to keep the class name");
                                                    string newClassName = Console.ReadLine();
                                                    if (!string.IsNullOrWhiteSpace(newClassName))
                                                    {
                                                        classes[i].Name = newClassName;
                                                    }
                                                    System.IO.File.WriteAllText(System.IO.Path.Combine(directory,$"{classes[i].Name}.cs"), classes[i].GetCSharp());
                                                    System.IO.File.WriteAllText(System.IO.Path.Combine(directory, $"{classes[i].Name}.sql"), classes[i].GetSQLTable());
                                                    System.IO.File.WriteAllText(System.IO.Path.Combine(directory, $"p{classes[i].Name}.sql"), classes[i].GetSQLStoredProcedure());
                                                }
                                            }
                                            
                                        }                                       
                                        else
                                        {
                                            Console.WriteLine("Could not find path please select a valid path ID.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Unable to parse input into integer.");
                                    }
                                }
                            } while (input != "b");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to get schema for version {input}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Version not found enter ? to see a list of valid versions");
                    }
                }
                Console.WriteLine();
            } while (input.ToLower() != "q");
            
        }

        public static async Task ShowVersions()
        {
            await GetVersions();
            PrintVersions(m_versions);
        }

        public static async Task GetVersions()
        {
            var task = Schema.ESISchema.GetSchemaVersions();
            await task;
            if (task.IsCompleted)
            {
                m_versions = task.Result;
            }
        }
        
        public static bool VersionExists(string version)
        {
            return m_versions.Contains(version);
        }

        public static void PrintVersions(List<string> versions)
        {
            Console.WriteLine($"{versions.Count} versions avaliable:");
            for (int i = 0; i < versions.Count; i++)
            {
                Console.WriteLine(versions[i]);
            }
        }
        
    }
}
