using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


[CreateAssetMenu(menuName="Data/ Paintings")]
public class PaintingsData : ScriptableObject
{
    const string resourcePath = "Assets/Resources/";
    [SerializeField] string infoFilePath = "Paintings/info.csv";
    
    List<Painting> paintings;
    public List<Texture> textures { get; private set; }

    
    public Painting[] Paintings
    {
        get{ return paintings.ToArray(); }
    }

    public Painting getNextPainting( Painting currentPainting )
    {
        if( currentPainting == null )
            return paintings[0];
        
        int current = paintings.BinarySearch( currentPainting );
        return paintings[(current+1) % paintings.Count];
    }


    private void Awake()
    {
        Debug.Log( "PaintingsData Awake" );
    }

    private void OnEnable()
    {
        Debug.Log( "PaintingsData Enable" );
        if( shouldReloadData() )
        {
            try
            {
                LoadAllPaintings();
            }
            catch (Exception exp)
            {
                Debug.LogError( exp.Message );

            }
        }
    }

    private bool shouldReloadData()
    {
        // TODO
        return true;
    }

    private void LoadAllPaintings()
    {
        LoadPaintings();
        DiscardPaintingsWithoutTexture();
    }

    private void LoadPaintings()
    {
        paintings = new List<Painting>();

        using( StreamReader infoFile = new StreamReader(resourcePath+infoFilePath) )
        {
            // currently assume this line would be what we want ( see: enum PropertiesToQuery )
            string[] keys = ReadOneRawOfCsv( infoFile );
            
            while( ! infoFile.EndOfStream )
            {
                string[] values = ReadOneRawOfCsv( infoFile );
                try
                {
                    Painting nextPainting = new Painting(values);
                    int lastIndex = paintings.Count-1;

                    if( paintings.Count > 0 && paintings[lastIndex].uid == nextPainting.uid )
                        paintings[lastIndex].Merge(nextPainting);
                    else
                        paintings.Add(nextPainting);
                }
                catch
                {
                    Debug.Log("Failed to Load item: "+values[0]);
                }
            }

            if( paintings.Count == 0 )
                throw new Exception("Nothing loaded by LoadPaintings()");

            paintings.Sort();
            
            // Debug
            foreach( Painting painting in paintings )
            {
                Debug.Log(painting.name + ", w:" + painting.width + ", h:" + painting.height);
            }
        }
    }

    private string[] ReadOneRawOfCsv( StreamReader stream )
    {
        return stream.ReadLine().Split(',');
    }

    private string ReadUntil( StreamReader input, char lastChar )
    {
        var stringBuffer = new System.Text.StringBuilder();

        while( input.Peek() != lastChar )
            stringBuffer.Append( input.Read() );

        return stringBuffer.ToString();

    }

    private void DiscardPaintingsWithoutTexture()
    {
        foreach(var painting in paintings)
        {
            if( painting.texture == null )
                paintings.Remove(painting);
        }
    }
    


    enum PropertiesToQuery
    {
        item, itemLabel, creatorLabel, inception,
        height, width, materialLabel, image
    }
 
    public class Painting : IComparable 
    {
        // The term "Label" in here means readable name.
        // See Wikidata Query Service for more information.
        public string uid { get; private set; }
        public string name { get; private set; }
        public string year { get; private set; }
        public float height { get; private set; }
        public float width { get; private set; }
        public List<string> creators { get; private set; }
        public List<string> materials { get; private set; }   
        public string imageURL { get; private set; }
        public Texture texture { get; private set; }
        

        public Painting( string[] data )
        {
            Debug.Log("Painting.Painting()");
            Init();

            uid = data[(int)PropertiesToQuery.item].Split('/')[4];

            name = data[(int)PropertiesToQuery.itemLabel];
            year = data[(int)PropertiesToQuery.inception].Substring(0,4);
            imageURL = data[(int)PropertiesToQuery.image];

            height = float.Parse( data[(int)PropertiesToQuery.height] );
            width = float.Parse( data[(int)PropertiesToQuery.width] );

            creators.Add( data[(int)PropertiesToQuery.creatorLabel] );
            materials.Add( data[(int)PropertiesToQuery.materialLabel] );
            
            texture = Resources.Load<Texture>( "Paintings/"+uid );
            if(texture==null)
                Debug.Log("Failed to load  "+uid);
        }

        public void Init()
        {
            uid = "";
            name = "";
            creators = new List<string>();
            year = "?";
            height = float.NaN;
            width = float.NaN;
            materials = new List<string>();
            imageURL = "";
        }

        public void Merge(Painting other)
        {
            if( uid != other.uid )
            {
                throw new System.Exception("Merging PaintingInfo with different uid");
            }
        
            creators.AddRange(other.creators);
            if( height == float.NaN )
                height = other.height;
            if( width == float.NaN )
                width = other.width;
            materials.AddRange(other.materials);
        }

        public int CompareTo(object obj)
        {
            Painting other = obj as Painting;
            return uid.CompareTo(other.uid);
        }


        public static bool operator < (Painting p1, Painting p2) => (p1.uid.CompareTo(p2.uid) < 0);
        public static bool operator > (Painting p1, Painting p2) => (p1.uid.CompareTo(p2.uid) > 0);
    }
}
