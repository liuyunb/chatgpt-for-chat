using UnityEngine;

[CreateAssetMenu(fileName = "CloseDialog", menuName = "Dialog/CloseDialog")]
public class EN_CloseDialog : EventNodeBase_SO
{
    public override void Execute()
    {
        base.Execute();
        UIManager.Instance.dialog.CloseDialog();
        OnFinished?.Invoke(true);
        Debug.Log(111);
    }
}