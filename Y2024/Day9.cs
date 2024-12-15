using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Y2024;

public class Day9 : Day
{
    private sealed record File(int Id, int Pos, int Size);
    
    private readonly Dictionary<int, int> blockToFileId = new();
    private readonly Dictionary<int, File> fileIdToFile = new();
    
    private int GetDiskSize()
    {
        this.blockToFileId.Clear();
        this.fileIdToFile.Clear();
        var diskMap = this.GetInputLines().First();
        
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

        return pos;
    }

    [ExpectedResult(1928L, 6200294120911)]
    public override object SolvePartOne()
    {
        var diskSize = this.GetDiskSize();
        this.DebugDiskMap(diskSize, "Before rearrange: ");

        var lastBlock = diskSize - 1;
        for (var block = 0; block < diskSize; block++)
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
        
        this.DebugDiskMap(diskSize, "After rearrange:  ");
        return this.CalculateChecksum(diskSize);
    }

    [ExpectedResult(2858L, 6227018762750)]
    public override object SolvePartTwo()
    {
        var diskSize = this.GetDiskSize();
        this.DebugDiskMap(diskSize, "Before rearrange: ");

        var files = this.fileIdToFile
            .OrderByDescending(e => e.Key)
            .Where(e => e.Key > 0)
            .Select(e => e.Value);
        
        foreach (var file in files)
        {
            var space = this.TryGetPositionOfFreeSpace(file);
            if (space == null) continue;
            
            var moved = file with { Pos = space.Value };
            this.fileIdToFile[file.Id] = moved; 
            for (var j = 0; j < file.Size; j++)
            {
                this.blockToFileId.Remove(file.Pos + j);
                this.blockToFileId[space.Value + j] = file.Id;
            }
        }
        
        this.DebugDiskMap(diskSize, "After rearrange:  ");
        return this.CalculateChecksum(diskSize);
    }

    private void DebugDiskMap(int diskSize, string prefix)
    {
        if (ExecutionContext.Mode != ExecutionMode.Debug) return;
        
        var sb = new StringBuilder(diskSize);
        for (var i = 0; i < diskSize; i++)
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

    private long CalculateChecksum(int diskSize)
    {
        long checksum = 0;
        for (var block = 0; block < diskSize; block++)
        {
            if (this.blockToFileId.TryGetValue(block, out var id))
            {
                checksum += id * block;
            }
        }

        return checksum;
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