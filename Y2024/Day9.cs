using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Y2024;

[Year(2024)]
public class Day9 : Day<long>
{
    private sealed record File(int Id, int Pos, int Size);
    
    private readonly Dictionary<int, int> blockToFileId = new();
    private readonly Dictionary<int, File> fileIdToFile = new();
    private int diskSize;
    
    private void Initialize()
    {
        this.blockToFileId.Clear();
        this.fileIdToFile.Clear();
        var diskMap = this.GetInputLines()[0];
        
        var id = 0;
        var isFile = true;
        var pos = 0;
        foreach (var c in diskMap)
        {
            var length = c - '0';
            if (isFile)
            {
                var file = new File(id, pos, length);
                this.fileIdToFile.Add(id, file);
                
                for (var i = 0; i < length; i++)
                {
                    this.blockToFileId.Add(pos, id);
                    pos++;
                }

                isFile = false;
                id++;
            }
            else
            {
                pos += length;
                isFile = true;
            }
        }

        this.diskSize = pos;
    }

    public override long SolvePartOne()
    {
        this.Initialize();
        this.DebugDiskMap("Before rearrange: ");

        var lastBlock = this.diskSize - 1;
        for (var block = 0; block < this.diskSize; block++)
        {
            if (this.blockToFileId.ContainsKey(block))
            {
                continue;
            }

            if (block == lastBlock) break;
            
            int id;
            while (!this.blockToFileId.Remove(lastBlock, out id))
            {
                lastBlock--;
            }

            this.blockToFileId.Add(block, id);
        }
        
        this.DebugDiskMap("After rearrange:  ");
        return CalculateChecksum();
    }

    private void DebugDiskMap(string prefix)
    {
        if (ExecutionContext.Mode != ExecutionMode.Debug) return;
        
        var sb = new StringBuilder(this.diskSize);
        for (var i = 0; i < this.diskSize; i++)
        {
            if (this.blockToFileId.TryGetValue(i, out var file))
            {
                sb.Append(file);
            }
            else
            {
                sb.Append('.');
            }
        }

        this.DebugOut(prefix + sb);
    }

    private long CalculateChecksum()
    {
        long checksum = 0;
        for (var block = 0; block < this.diskSize; block++)
        {
            if (this.blockToFileId.TryGetValue(block, out var id))
            {
                checksum += id * block;
            }
        }

        return checksum;
    }

    public override long SolvePartTwo()
    {
        this.Initialize();
        this.DebugDiskMap("Before rearrange: ");

        var files = this.fileIdToFile
            .OrderByDescending(e => e.Key)
            .Where(e => e.Key > 0)
            .Select(e => e.Value);
        
        foreach (var file in files)
        {
            var space = TryGetPositionOfFreeSpace(file);
            if (space == null) continue;
            
            var moved = file with { Pos = space.Value };
            this.fileIdToFile[file.Id] = moved; 
            for (var j = 0; j < file.Size; j++)
            {
                this.blockToFileId.Remove(file.Pos + j);
                this.blockToFileId[space.Value + j] = file.Id;
            }
        }
        
        this.DebugDiskMap("After rearrange:  ");
        return CalculateChecksum();
    }

    private int? TryGetPositionOfFreeSpace(File file)
    {
        var size = 0;
        for (var block = 0; block <= file.Pos; block++)
        {
            if (size == file.Size)
            {
                return block - size;
            }
            
            if (this.blockToFileId.ContainsKey(block))
            {
                size = 0;
                continue;
            }

            size++;
        }

        return null;
    }

    protected override string GetTestInput(int? part = null) => "2333133121414131402";
}