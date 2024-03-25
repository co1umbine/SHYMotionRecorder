# SHYMotionRecorder

[EasyMotionRecorder](https://github.com/neon-izm/EasyMotionRecorder)を元にした、人型モーションの記録、再生、編集をするためのライブラリです。

人型モーションの記録、記録データの切り出し、CSV出力、CSYデータの再生ができます。

## 使い方

Unityのパッケージマネージャーから、Git URL(末尾が.git)を指定してインストールしてください。

### セットアップ

Sampleにセットアップ済みのシーンがあります。

1. 記録したいモーションを再生するHumanoidキャラクターをシーンに配置する。
2. SHYMotionCaptureプレハブをシーンに配置する。
3. SHYMotionCaptureプレハブのMotionDataRecorderやMotionDataPlayerのAnimatorにHumanoidキャラクターを設定する。

### モーション記録

1. Unityエディタ上で実行し、Rキーを押して記録を開始、Xキーで記録を終了しファイル書き出しをする。
2. 書き出されたファイルはAssets/Resources/に保存される。

ScriptableObject形式もしくはCSV形式で記録ができます。

### モーション再生

1. モーション記録で書き出されたファイルをMotionDataPlayerに設定する。
2. Unityエディタ上で実行し、Sキーを押して再生を開始、Tキーで再生を終了する。

CSV形式のデータはMotionDataPlayerCSVで再生してください。

### モーション切り出し

Sampleにセットアップ済みのシーンがあります。

MotionDataEditorを使用して、モーションの開始フレームから現在フレームまでを削除もしくは保存ができます。削除や保存ごとに操作履歴を保存し、編集前に戻すことが出来ます。メモリを消費するので操作履歴をいくつまで保存するか指定してください。

編集した後、Unity実行を終了もしくはResetを押すと開始時の状態に戻ります。

EditorCanvasプレハブを使用することでGUIで編集ができます。EditorCanvasプレハブをシーンに配置し、EditorとPlayerを指定してください。モーションを同じ長さで切り出す際にはMotionDataEditorUIの代わりにFixedFrameTrimmerUIを使用すると便利です。

ScriptableObject形式もしくはCSV形式で切り出しができます。

### CSV(SHY形式)出力

MotionDataRecorderでの記録、MotinoDataEditorでの切り出し時にCSV出力を行うことが出来ます。また、ScriptableObjectのコンテキストメニュー（ScriptableObjectを選択し、Inspectorウィンドウでデータの名前を右クリック）からもCSV出力が行えます。

CSV出力した後でも再生や切り出しはできますが、情報量が落ちたりファイル読み込みに時間がかかるので、CSV出力は最後に行うことをお勧めします。

## SHY形式について

Unity humanoid motion data for machine learning.

FPS 30

Per Frame: 467 columns

## Root Position and Rotation (Quaternion)

7 items

```
BodyPosition.x, BodyPosition.y, BodyPosition.z, BodyRotation.x, BodyRotation.y, BodyRotation.z, BodyRotation.w,
```

## Muscle value, conforming to the item/order of Unity HumanTrait.MuscleName. Excluding jaw and eyes. Muscle value range is normaly [-1, 1], but not exact

89  (95 - 4 eyes muscles - 2 jaw muscles)  items

```
Spine Front-Back, Spine Left-Right, Spine Twist Left-Right, Chest Front-Back, Chest Left-Right, Chest Twist Left-Right, UpperChest Front-Back, UpperChest Left-Right, UpperChest Twist Left-Right, Neck Nod Down-Up, Neck Tilt Left-Right, Neck Turn Left-Right, Head Nod Down-Up, Head Tilt Left-Right, Head Turn Left-Right, Left Upper Leg Front-Back, Left Upper Leg In-Out, Left Upper Leg Twist In-Out, Left Lower Leg Stretch, Left Lower Leg Twist In-Out, Left Foot Up-Down, Left Foot Twist In-Out, Left Toes Up-Down, Right Upper Leg Front-Back, Right Upper Leg In-Out, Right Upper Leg Twist In-Out, Right Lower Leg Stretch, Right Lower Leg Twist In-Out, Right Foot Up-Down, Right Foot Twist In-Out, Right Toes Up-Down, Left Shoulder Down-Up, Left Shoulder Front-Back, Left Arm Down-Up, Left Arm Front-Back, Left Arm Twist In-Out, Left Forearm Stretch, Left Forearm Twist In-Out, Left Hand Down-Up, Left Hand In-Out, Right Shoulder Down-Up, Right Shoulder Front-Back, Right Arm Down-Up, Right Arm Front-Back, Right Arm Twist In-Out, Right Forearm Stretch, Right Forearm Twist In-Out, Right Hand Down-Up, Right Hand In-Out, Left Thumb 1 Stretched, Left Thumb Spread, Left Thumb 2 Stretched, Left Thumb 3 Stretched, Left Index 1 Stretched, Left Index Spread, Left Index 2 Stretched, Left Index 3 Stretched, Left Middle 1 Stretched, Left Middle Spread, Left Middle 2 Stretched, Left Middle 3 Stretched, Left Ring 1 Stretched, Left Ring Spread, Left Ring 2 Stretched, Left Ring 3 Stretched, Left Little 1 Stretched, Left Little Spread, Left Little 2 Stretched, Left Little 3 Stretched, Right Thumb 1 Stretched, Right Thumb Spread, Right Thumb 2 Stretched, Right Thumb 3 Stretched, Right Index 1 Stretched, Right Index Spread, Right Index 2 Stretched, Right Index 3 Stretched, Right Middle 1 Stretched, Right Middle Spread, Right Middle 2 Stretched, Right Middle 3 Stretched, Right Ring 1 Stretched, Right Ring Spread, Right Ring 2 Stretched, Right Ring 3 Stretched, Right Little 1 Stretched, Right Little Spread, Right Little 2 Stretched, Right Little 3 Stretched
```

## Unity HumanBodyBone Global Position and Rotation. Conforming to the item/order of Unity HumanTrait.BoneName. Excluding eyes, jaw and last bone

52 humanoid bones (56 - 2eyes - 1jaw - 1lastBone) * 7 items
```
Hips, LeftUpperLeg, RightUpperLeg, LeftLowerLeg, RightLowerLeg, LeftFoot, RightFoot, Spine, Chest, Neck, Head, LeftShoulder, RightShoulder, LeftUpperArm, RightUpperArm, LeftLowerArm, RightLowerArm, LeftHand, RightHand, LeftToes, RightToes, LeftThumbProximal, LeftThumbIntermediate, LeftThumbDistal, LeftIndexProximal, LeftIndexIntermediate, LeftIndexDistal, LeftMiddleProximal, LeftMiddleIntermediate, LeftMiddleDistal, LeftRingProximal, LeftRingIntermediate, LeftRingDistal, LeftLittleProximal, LeftLittleIntermediate, LeftLittleDistal, RightThumbProximal, RightThumbIntermediate, RightThumbDistal, RightIndexProximal, RightIndexIntermediate, RightIndexDistal, RightMiddleProximal, RightMiddleIntermediate, RightMiddleDistal, RightRingProximal, RightRingIntermediate, RightRingDistal, RightLittleProximal, RightLittleIntermediate, RightLittleDistal, UpperChest
```

For Each Bone
```
Position.x, Position.y, Position.z, Quaternion.x, Quaternion.y, Quaternion.z, Quaternion.w,
```

## Whether the feet are grounded. Defind through foot height and velocity under certain thresholds. Indicates that 1 is grounded

4 items
```
leftfoot0/1, lefttoe0/1
rightfoot0/1, righttoe0/1
```

## Calculated by the difference from the next frame

3 items

### Velocity. Difference of position.  discussion: Unity Animator has velocity property

```
Root Velocity.x, Velosity.z
```

### Yaw axis angular velocity. Euler

```
Yaw Angluar velocity (Euler y)
```
