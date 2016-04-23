using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[ExecuteInEditMode]
public class MatchSizeToSprite : MonoBehaviour {

     Vector3 spriteSize;
     Vector2 theScale;
     Vector3 theSize;
    Vector3 targetSize;
     BoxCollider2D theBox;
    SpriteRenderer theRenderer;
    Transform myTransform;

  
    // Use this for initialization
    void Start () {
      
        //assign reference variables
        myTransform = GetComponent<Transform>();
        theBox = GetComponent<BoxCollider2D>();
        theRenderer = GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
        
        spriteSize = theRenderer.bounds.size;
      
        theScale = myTransform.transform.localScale;
        theSize.x = spriteSize.x / theScale.x;
        theSize.y = spriteSize.y / theScale.y;
        theBox.size = theSize;

        //Debug.Log("spriteSize = " + spriteSize + "theScale = " + theScale);

    }
}
