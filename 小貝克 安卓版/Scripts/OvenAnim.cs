using UnityEngine;

public class OvenAnim : MonoBehaviour
{
    private MakeCake makeCake;
    private MakeDonut makeDonut;
    private MakeBread makeBread;
    private MakeCupCake makeCupCake;
    private MakeStyleBread makeStyleBread;
    public FoodType currentFoodType; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject makeCakeObject = GameObject.Find("MakeCakeScript");
        if (makeCakeObject != null)
        {
            makeCake = makeCakeObject.GetComponent<MakeCake>();
        }
        GameObject makeDonutObject = GameObject.Find("MakeDonutScript");
        if (makeDonutObject != null)
        {
            makeDonut = makeDonutObject.GetComponent<MakeDonut>();
        }
        GameObject makeBreadObject = GameObject.Find("MakeBreadScript");
        if (makeBreadObject != null)
        {
            makeBread = makeBreadObject.GetComponent<MakeBread>();
        }
        GameObject makeCupCakeObject = GameObject.Find("MakeCupCakeScript");
        if (makeCupCakeObject != null)
        {
            makeCupCake = makeCupCakeObject.GetComponent<MakeCupCake>();
        }
        GameObject makeStyleBreadObject = GameObject.Find("MakeStyleBreadScript");
        if (makeStyleBreadObject != null)
        {
            makeStyleBread = makeStyleBreadObject.GetComponent<MakeStyleBread>();
        }
    }

    void Update()
    {
        
    }

    public void SetCurrentFoodType(FoodType foodType)
    {
        currentFoodType = foodType;
    }

    public void OvenAnimComplete()
    {
        if (currentFoodType == FoodType.Cake && makeCake != null)
        {
            makeCake.OvenAnimationComplete(currentFoodType);
            makeCake.SmokeParticle.SetActive(false);
            makeCake.RedLight.SetActive(false);
            makeCake.isAnimationPlaying = false;
        }
        else if (currentFoodType == FoodType.Donut && makeDonut != null)
        {
            makeDonut.OvenAnimationComplete(currentFoodType);
            makeDonut.SmokeParticle.SetActive(false);
            makeDonut.RedLight.SetActive(false);
            makeDonut.isAnimationPlaying = false;
        }
        else if (currentFoodType == FoodType.Bread && makeBread != null)
        {
            makeBread.OvenAnimationComplete(currentFoodType);
            makeBread.SmokeParticle.SetActive(false);
            makeBread.RedLight.SetActive(false);
            makeBread.isAnimationPlaying = false;
        }
        else if (currentFoodType == FoodType.CupCake && makeCupCake != null)
        {
            makeCupCake.OvenAnimationComplete(currentFoodType);
            makeCupCake.SmokeParticle.SetActive(false);
            makeCupCake.RedLight.SetActive(false);
            makeCupCake.isAnimationPlaying = false;
        }
        else if (currentFoodType == FoodType.StyleBread && makeStyleBread != null)
        {
            makeStyleBread.OvenAnimationComplete(currentFoodType);
            makeStyleBread.SmokeParticle.SetActive(false);
            makeStyleBread.RedLight.SetActive(false);
            makeStyleBread.isAnimationPlaying = false;
        }
    }


}
