using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileInfo
{
    private string fileId;
    private string fileName;
    private string extension;
    private string uploadUser;
    private byte[] content;

    public FileInfo(string fileId, string fileName, string extension, string uploadUser, byte[] content)
    {
        this.fileId = fileId;
        this.fileName = fileName;
        this.extension = extension;
        this.uploadUser = uploadUser;
        this.content = content;
    }

    public string GetFileId()
    {
        return this.fileId;
    }

    public string GetFileName()
    {
        return this.fileName;
    }

    public string GetExtension()
    {
        return this.extension;
    }

    public string GetUploadUser()
    {
        return this.uploadUser;
    }

    public byte[] GetContent()
    {
        return this.content;
    }
}
