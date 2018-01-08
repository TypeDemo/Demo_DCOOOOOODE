using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBuildItem : BuildItem {

    protected override void FinishBuild()
    {
        if (Build.buildBox.ContainsKey(Build.buildTransforms[hz, vt]) || !KnapSackManager.instance.CastItem(10001))
            return;
        //转换控制对象，激活和关闭相关组件
        controller.controlState = GameObjectController.ControlState.Player;
        isBuilded = true;
       
        GetComponent<Collider>().isTrigger = false;
        GetComponent<BuildItem>().enabled = false;
        buildSys.SetActive(false);

        Build.buildBox.Add(Build.buildTransforms[hz, vt], this.gameObject);

        //从按钮中移除自己的事件不再被调用
        finishButton.onClick.RemoveListener(FinishBuild);
        finishButton.gameObject.SetActive(false);
        buildButton.SetActive(true);
        cancelButton.onClick.RemoveListener(CancelBuild);
        cancelButton.gameObject.SetActive(false);


    }
}
