using System.IO.Abstractions;
using System.Text;
using CSharpier.Cli;
using CSharpier.SyntaxPrinter;
using CSharpier.Utilities;
using DiffEngine;
using FluentAssertions;
using Microsoft.CodeAnalysis;

namespace CSharpier.Tests.FormattingTests;

public class BaseTest
{
    private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent("CSharpier.Tests");

    protected async Task RunTest(
        string fileName,
        string fileExtensionWithoutDot,
        bool useTabs = false
    )
    {
        var filePath = Path.Combine(
            this.rootDirectory.FullName,
            "FormattingTests",
            "TestFiles",
            fileExtensionWithoutDot,
            fileName + ".test"
        );
        var fileReaderResult = await FileReader.ReadFileAsync(
            filePath,
            new FileSystem(),
            CancellationToken.None
        );

        var result = await CodeFormatter.FormatAsync(
            fileReaderResult.FileContents,
            "." + fileExtensionWithoutDot,
            new PrinterOptions { Width = PrinterOptions.WidthUsedByTests, UseTabs = useTabs },
            // TODO fileExtensionWithoutDot.EqualsIgnoreCase("csx") ? SourceCodeKind.Script : SourceCodeKind.Regular,
            CancellationToken.None
        );

        var actualFilePath = filePath.Replace(".test", ".actual.test");
        await File.WriteAllTextAsync(actualFilePath, result.Code, fileReaderResult.Encoding);

        var filePathToChange = filePath;
        var expectedFilePath = actualFilePath.Replace(".actual.", ".expected.");

        var expectedCode = fileReaderResult.FileContents;

        if (File.Exists(expectedFilePath))
        {
            expectedCode = await File.ReadAllTextAsync(expectedFilePath, Encoding.UTF8);
            filePathToChange = expectedFilePath;
        }

        var normalizedCode = result.Code;

        if (Environment.GetEnvironmentVariable("NormalizeLineEndings") != null)
        {
            expectedCode = expectedCode.Replace("\r\n", "\n");
            normalizedCode = normalizedCode.Replace("\r\n", "\n");
        }

        var comparer = new SyntaxNodeComparer(
            expectedCode,
            normalizedCode,
            false,
            false,
            SourceCodeKind.Regular,
            CancellationToken.None
        );

        result.CompilationErrors.Should().BeEmpty();
        result.FailureMessage.Should().BeEmpty();

        if (normalizedCode != expectedCode && !BuildServerDetector.Detected)
        {
            await DiffRunner.LaunchAsync(filePathToChange, actualFilePath);
        }
        normalizedCode.Should().Be(expectedCode);

        var compareResult = comparer.CompareSource();
        compareResult.Should().BeNullOrEmpty();
    }
}
