using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class DrawingManager : MonoBehaviour {

	//Line的Prefab，是我们要生成的线条。
	public GameObject lineOBJ;
	//记录AR摄像头上一帧的位置。
	private Vector3 previousPosition;
	//标记当前是否在画画。
	private bool isDrawing = true;
	//记录当前正在使用的Line Object。
	private Line _currentLine;

	//Unity 方法
	#region MONO CALLBACKS
		//当此component被启用时。
		void OnEnable()
		{
			//注册AR帧识别的处理办法。
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
			//【测试用】创建一条当前使用的line。
			_currentLine = Instantiate(lineOBJ, this.transform).GetComponent<Line>();
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

	//当手机端识别出一个AR帧之后，我们要用以下方法进行处理。
	//可以理解为这个参数camera就是我们的手机摄像头，但是要转换到unity的坐标系。
	//见getCamPos
    public void ARFrameUpdated(UnityARCamera camera)
    {	
		//得到当前相机位置时，我们要稍微往foward方向加一些距离，
		//这样可以确保线一直在相机前面，为了视觉效果。
        Vector3 currentPositon =  getCamPos(camera) + (Camera.main.transform.forward * 0.2f);
        if (Vector3.Distance (currentPositon, previousPosition) > 0.02f) {
		
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
}
