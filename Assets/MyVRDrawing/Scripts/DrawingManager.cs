using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour {

	//Line的Prefab，是我们要生成的线条。
	public GameObject lineOBJ;
	//开始按钮文本。
	public Text startButtonText;
	//颜色按键的文本, 012分别对应红绿蓝按键文本。
	public Text[] colorButtonTexts;

	//记录AR摄像头上一帧的位置。
	private Vector3 previousPosition;
	//标记当前是否在画画。
	private bool isDrawing = false;
	//记录当前正在使用的Line Object。
	private Line _currentLine;
	//记录当前使用的颜色。
	private Color _currentColor;

	//Unity 方法
	#region MONO CALLBACKS
		//当此component被启用时。
		void OnEnable()
		{
			//注册AR帧识别的处理办法。
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
			//默认开始使用红色线，这句话也会为currentline创建一条新的线。
			ChangeLineColor(0);
		}
		//当此component被删除时。
		void OnDestroy()
		{
			//当消失时，我们要分离这个我们曾经注册上去的方法。
			//【 要不然会有bug，想象一下，ARFrameUpdatedEvent还会每一帧通知它的listener但是我们这个function由于
			//被删除却不复存在了。因此不做此操作会造成问题。】
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdated;
		}


	#endregion

	//AR方法
	#region AR CALLBACK
	//当手机端识别出一个AR帧之后，我们要用以下方法进行处理。
	//可以理解为这个参数camera就是我们的手机摄像头，但是要转换到unity的坐标系。
	//见getCamPos
    public void ARFrameUpdated(UnityARCamera camera)
    {	
		//得到当前相机位置时，我们要稍微往foward方向加一些距离，
		//这样可以确保线一直在相机前面，为了视觉效果。
        Vector3 currentPositon =  getCamPos(camera) + (Camera.main.transform.forward * 0.2f);
        if (Vector3.Distance (currentPositon, previousPosition) > 0.01f) {
		
		//判断如果在画画的话，我们持续把相机位置加进line中。
		//Line会处理与LineRenderer的操作，用来更新画出的线条。详见Line。
		if(isDrawing)
		{
			//给当前line在当前位置上加一个点。
			_currentLine.AddPoint(currentPositon);
		}

			//更新上一次位置。
            previousPosition = currentPositon;
        }
    }
	#endregion

	//其他public方法
	#region PUBLIC 方法
	//用来处理画画状态和UI显示状态。
	//如果是未开始状态的话，我们要让开始键显示“开始画”，
	//反之如果是已经开始状态的话，我们要让开始键显示“结束画”。
	//详见EndDrawing 和 StartDrawing.
	public void SwitchStartButton()
	{
		if(isDrawing)
		{
			EndDrawing();			
		}
		else
			StartDrawing();
	}

	//由按钮控制的方法，用来改变画线的颜色，同时更新UI
	//我们要重新建一条线是以为我们要维持之前线的颜色。
	//这里使用int 来表示颜色，为了方便更新ui。
	//0 是红色， 1是绿色，2是蓝色
	public void ChangeLineColor(int colorIndex)
	{
		//根据colorindex值找到要用的颜色.
		switch(colorIndex)
		{
		case 0:
			_currentColor = Color.red;
			break;
		case 1:
			_currentColor = Color.green;
			break;
		case 2:
			_currentColor = Color.blue;
			break;
		default:
			_currentColor = Color.red;
			break;
		}
		//显示按键文本（一个对号），用来体现这个颜色被选中。
		for(int i=0; i<colorButtonTexts.Length; i++)
		{
			if(i == colorIndex)
			{
				colorButtonTexts[i].gameObject.SetActive(true);				
			}
			else
				colorButtonTexts[i].gameObject.SetActive(false);
		}
		//如果当前的线是空的的话或者它含有点点数大于0，我们就建一个新的线。
		//这个判断确保了我们不会在任意切换颜色时创建多余的line。
		if(_currentLine == null || _currentLine.points.Count != 0)
			_currentLine = Instantiate(lineOBJ, this.transform).GetComponent<Line>();
		_currentLine.SetColor(_currentColor);	
	}
	#endregion

	//计算出摄像头位置，其位置就是用户摄像头在世界里的位置。
	//我们用这个办法来确定摄像头位置，然后生成组成画的点。
	private Vector3 getCamPos(UnityARCamera camera)
	{
		//创建一个新4X4矩阵
        Matrix4x4 matrix = new Matrix4x4();
		//第4列设置成位置。
        matrix.SetColumn(3, camera.worldTransform.column3);
		//返回转换过的相机位置。
		return UnityARMatrixOps.GetPosition(matrix);
	}

	//由按钮控制的方法，用来开始画画。
	private void StartDrawing()
	{
		//标记画画已开始
		isDrawing = true;
		//更新UI。
		startButtonText.text = "停止画";
		//创建一条新的线。
		_currentLine = Instantiate(lineOBJ, this.transform).GetComponent<Line>();
		//保持现在选择的颜色。
		_currentLine.SetColor(_currentColor);
	}
	//由按钮控制的方法，用来结束画画。
	private void EndDrawing()
	{
		//更新UI。
		startButtonText.text = "开始画";
		isDrawing = false;		
	}

}
