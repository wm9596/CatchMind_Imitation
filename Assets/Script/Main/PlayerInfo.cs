using System.Collections;
using System.Collections.Generic;


public class PlayerInfo 
{
    private string name;
    private string fileName;

    public PlayerInfo(string _name,string _fileName)
    {
        name = _name;
        fileName = _fileName;
    }

    public void SetName(string _name)
    {
        name = _name;
    }

    public void SetImgPath(string _fileName)
    {
        fileName = _fileName;
    }

    public string GetName()
    {
        return name;
    }
    
    public string GetFileName()
    {
        return fileName;
    }

}
