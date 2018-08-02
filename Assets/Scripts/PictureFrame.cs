using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PictureFrame : MonoBehaviour
{
    [SerializeField] GameObject frameTop;
    [SerializeField] GameObject frameDown;
    [SerializeField] GameObject frameLeft;
    [SerializeField] GameObject frameRight;
    [SerializeField] GameObject picture;
    [SerializeField] Projector shadowProjector;
    [SerializeField] float frameThickness = 0.05f;

    [SerializeField] PaintingsData paintingsData;
    private PaintingsData.Painting painting;
    

    private void Start ()
    {
        Debug.Log("PictureFrame Start");

        ChangePicture();
	}

    private void OnMouseDown()
    {
        ChangePicture();
    }

    private void ChangePicture()
    {
        Debug.Log("Change Picture");
        painting = paintingsData.getNextPainting( painting );
        if( painting == null )
        {
            Debug.LogError("paintingsData.getNextPainting() Failed");
            return;
        }

        picture.GetComponent<Renderer>().material.SetTexture("_MainTex", painting.texture);
        AdjustScale();
    }
    

    private void AdjustScale()
    {
        float pictureWidth = picture.GetComponent<Renderer>().
                                material.GetTexture("_MainTex").width*0.001f;
        float pictureHeight = picture.GetComponent<Renderer>().
                                material.GetTexture("_MainTex").height*0.001f;

        AdjustFrameScale();
        AdjustShadowProjectorSize();
    }

    private void AdjustFrameScale( )
    {
        /*  Adjust size of picture & frame
         *  
         *    When looking at the panting : 
         *  
         *           ====Top====
         *           ||       ||
         *      Left ||  Pic  || Right
         *           ||       ||
         *           ====Down===
        */
        
        // Change cm to m
        float width = painting.width*0.01f;
        float height = painting.height*0.01f;

        picture.transform.localScale = new Vector3(width, height, frameThickness*0.5f);

        frameTop.transform.localPosition  = new Vector3(0, height*0.5f+frameThickness*0.5f, frameThickness*0.5f);
        frameDown.transform.localPosition = new Vector3(0, -(height*0.5f+frameThickness*0.5f), frameThickness*0.5f);
        frameTop.transform.localScale  = new Vector3(width+frameThickness*2, frameThickness, frameThickness);
        frameDown.transform.localScale = new Vector3(width+frameThickness*2, frameThickness, frameThickness);

        frameLeft.transform.localPosition  = new Vector3(width*0.5f+frameThickness*0.5f, 0, frameThickness*0.5f);
        frameRight.transform.localPosition = new Vector3(-(width*0.5f+frameThickness*0.5f), 0, frameThickness*0.5f);
        frameLeft.transform.localScale  = new Vector3(frameThickness, height, frameThickness);
        frameRight.transform.localScale = new Vector3(frameThickness, height, frameThickness);

    }

    private void AdjustShadowProjectorSize( )
    {
         float width = painting.width*0.01f;
         float height = painting.height*0.01f;
        if(shadowProjector != null)
        {
            shadowProjector.aspectRatio = width/height;
            shadowProjector.orthographicSize = height*0.75f;
            shadowProjector.transform.localPosition = new Vector3(0,-0.1f*height,1);
        }
    }

}
