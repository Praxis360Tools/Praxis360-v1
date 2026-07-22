using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Praxis360_v1.Application.Constants;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;

namespace Praxis360_v1.Infrastructure.FileReaders;

public sealed class BrioCsvFileReader : IBrioFileReader
{
    private const char Delimiter = ';';
    private const char Quote = '"';
    private const int ExpectedColumns = BrioColumnPositions.ExpectedColumnCount;

    public async Task<BrioFileReadResult> ReadAsync(Stream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        var lines = new List<BrioRawLine>();
        var errors = new List<string>();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        var lineNumber = 0;
        var firstLine = await reader.ReadLineAsync();
        lineNumber++;

        if (firstLine is null)
        {
            errors.Add("File is empty.");
            return new BrioFileReadResult(lines, errors, ExpectedColumns);
        }

        var headerCells = ParseCsvLine(firstLine);

        if (headerCells.Count != ExpectedColumns)
        {
            errors.Add($"Header must contain exactly {ExpectedColumns} columns, but found {headerCells.Count}.");
            return new BrioFileReadResult(lines, errors, ExpectedColumns);
        }

        while (true)
        {
            var rawLine = await reader.ReadLineAsync();
            lineNumber++;

            if (rawLine is null)
                break;

            if (IsEmptyLine(rawLine))
                continue;

            var cells = ParseCsvLine(rawLine);

            if (cells.Count != ExpectedColumns)
            {
                errors.Add($"Line {lineNumber}: expected {ExpectedColumns} columns but found {cells.Count}.");
                continue;
            }

            lines.Add(new BrioRawLine(lineNumber, cells));
        }

        return new BrioFileReadResult(lines, errors, ExpectedColumns);
    }

    private static bool IsEmptyLine(string line)
    {
        return string.IsNullOrWhiteSpace(line);
    }

    private static List<string> ParseCsvLine(string line)
    {
        var cells = new List<string>();
        var currentCell = new StringBuilder();
        var insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var current = line[i];

            if (current == Quote)
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == Quote)
                {
                    currentCell.Append(Quote);
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (current == Delimiter && !insideQuotes)
            {
                cells.Add(currentCell.ToString());
                currentCell.Clear();
            }
            else
            {
                currentCell.Append(current);
            }
        }

        cells.Add(currentCell.ToString());

        return cells;
    }
}
