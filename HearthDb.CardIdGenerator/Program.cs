#region

using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

#endregion

namespace HearthDb.CardIdGenerator
{
	internal class Program
	{
		private const string Dir = "../../../../HearthDb/";

		static void Main()
		{
			var header = ParseLeadingTrivia(@"/* THIS CLASS WAS GENERATED BY HearthDb.CardIdGenerator. DO NOT EDIT. */");

			var changes = 0;

			var decls = SyntaxBuilder.GetCollectible().Concat(SyntaxBuilder.GetNonCollectible());
			foreach (var (name, collectible) in decls)
			{
				var cCardIds = ClassDeclaration("CardIds")
					.AddModifiers(Token(PublicKeyword), Token(PartialKeyword))
					.WithLeadingTrivia(header)
					.AddMembers(collectible);
				var ns = NamespaceDeclaration(IdentifierName("HearthDb")).AddMembers(cCardIds);
				var fileName = $"CardIds.{name}.cs";
				var path = Dir + fileName;
				Console.Write($"Formatting {fileName} namespace...");
				var root = Formatter.Format(ns, new AdhocWorkspace());
				var rootString = root.ToString();

				string prevString = "";
				if (File.Exists(path))
				{
					using (var sr = new StreamReader(path))
						prevString = sr.ReadToEnd();
				}

				if (prevString != rootString)
				{
					changes++;
					Console.WriteLine($" changed. Writing to disk.");
					using(var sr = new StreamWriter(path))
						sr.Write(root.ToString());
				}
				else
					Console.WriteLine($" no changes.");
				}

			Console.WriteLine($"{changes} files changed. Press any key to exit.");

			Console.ReadKey();
		}
	}
}
