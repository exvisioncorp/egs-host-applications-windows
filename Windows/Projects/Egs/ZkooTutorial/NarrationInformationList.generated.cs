namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    class NarrationInformationList
    {
        /// <summary>ZKOOに顔を認識させてください。 (Please face ZKOO and let it detect your face.)</summary>
        public NarrationInformation View100_Message00100 { get; private set; }
        /// <summary>ZKOOが顔を認識できません。顔と上半身がカメラビューに写るように、ZKOOの正面に座ってください。 (ZKOO cannot detect your face.  Make sure your face and upper body are positioned in the center of the Camera View.)</summary>
        public NarrationInformation View100_Message00200 { get; private set; }
        /// <summary>カメラビューが表示されない場合は、もう一度付属のSetup Guideをご覧になって、ZKOOの設置を確認してください。 (If you do not see the Camera View, refer to the Setup Guide on how to set up your ZKOO.)</summary>
        public NarrationInformation View100_Message00300 { get; private set; }
        /// <summary>ZKOOに手を認識させてください。 (Move your hand inside the Gesture Tracking Area.)</summary>
        public NarrationInformation View100_Message00400 { get; private set; }
        /// <summary>顔の左右に、ジェスチャートラッキングエリアの四角い枠が表示されます。 (You will see 2 rectangular outlines appear next to your face.  This represents the Gesture Tracking Areas.)</summary>
        public NarrationInformation View100_Message00500 { get; private set; }
        /// <summary>ジェスチャートラッキングエリア内の、リラックスできる場所に手を挙げてください。 (Move your hand inside the Gesture Tracking Area.  Make sure your arms are in a comfortable, relaxed position.)</summary>
        public NarrationInformation View100_Message00600 { get; private set; }
        /// <summary>指の間を軽く開けるようにして、手を左右に少し動かしてください。 (Open your hand and spread your fingers.  Slightly wave your hand.)</summary>
        public NarrationInformation View100_Message00700 { get; private set; }
        /// <summary>ジェスチャー操作を終了してください。 (Now, move your hand out of the Gesture Tracking Area to end the gesture control.)</summary>
        public NarrationInformation View100_Message00800 { get; private set; }
        /// <summary>では、練習してみましょう。 (Let's practice.)</summary>
        public NarrationInformation View101_Message00000 { get; private set; }
        /// <summary>うまくできました。 (Well done.)</summary>
        public NarrationInformation View101_Message00100 { get; private set; }
        /// <summary>それでは、この練習を1分以内に5回行ってください。 (Let's try this 5 more times.  You have 1 minute.)</summary>
        public NarrationInformation View101_Message00215 { get; private set; }
        /// <summary>それでは、この練習を2分以内に5回行ってください。 (Let's try this 5 more times.  You have 2 minute.)</summary>
        public NarrationInformation View101_Message00225 { get; private set; }
        /// <summary>では始めてください。 (Go ahead.)</summary>
        public NarrationInformation View101_Message00600 { get; private set; }
        /// <summary>あと1回です。 (One more time.)</summary>
        public NarrationInformation View101_Message00201 { get; private set; }
        /// <summary>あと2回です。 (Two more times.)</summary>
        public NarrationInformation View101_Message00202 { get; private set; }
        /// <summary>あと3回です。 (Three more times.)</summary>
        public NarrationInformation View101_Message00203 { get; private set; }
        /// <summary>あと4回です。 (Four more times.)</summary>
        public NarrationInformation View101_Message00204 { get; private set; }
        /// <summary>タイムオーバーです。もう一度練習してみましょう。 (Time's up.  Please try again.)</summary>
        public NarrationInformation View101_Message00300 { get; private set; }
        /// <summary>うまくできました。次の練習に進みます。 (Well done.  Let's practice something else.)</summary>
        public NarrationInformation View101_Message00500 { get; private set; }
        /// <summary>ジェスチャーカーソルをスクリーンに表示させましょう。 (Let's practice displaying the Gesture Cursor on the screen.)</summary>
        public NarrationInformation View001_Message00100 { get; private set; }
        /// <summary>顔と上半身がカメラビューに写るように、ZKOOの正面に座ってください。 (Sit in front of the ZKOO and ensure both your face and upper body are positioned in the center of the Camera View.)</summary>
        public NarrationInformation View001_Message00200 { get; private set; }
        /// <summary>ZKOOが顔を認識しました。 (ZKOO has detected your face.)</summary>
        public NarrationInformation View001_Message00300 { get; private set; }
        /// <summary>もう一度ビデオを見る場合は [Video] をマウスかジェスチャーでクリックしてください。 (Click on the "Video" button by using a mouse or gesture to replay the Tutorial Video.)</summary>
        public NarrationInformation View001_Message00400 { get; private set; }
        /// <summary>この練習を再び行うには [Practice] をマウスかジェスチャーでクリックしてください。 (Click on the "Practice" button by using a mouse or gesture to retry this practice.)</summary>
        public NarrationInformation View001_Message00500 { get; private set; }
        /// <summary>この練習をスキップするには [Next] をマウスかジェスチャーでクリックしてください。 (Click on the "Next" button by using a mouse or gesture to skip this practice.)</summary>
        public NarrationInformation View001_Message00600 { get; private set; }
        /// <summary>ZKOOがあなたの手を認識し、手のトラッキングを開始しました。 (ZKOO has detected your hand and is now tracking it.)</summary>
        public NarrationInformation View001_Message00900 { get; private set; }
        /// <summary>ジェスチャートラッキングエリアの形が変わります。 (After ZKOO detects your hand, the shape of the Gesture Tracking Area changes. )</summary>
        public NarrationInformation View001_Message01000 { get; private set; }
        /// <summary>手のひらの中心に、ポインターが表示されます。 (A Pointer is shown at the center of your palm. )</summary>
        public NarrationInformation View001_Message01100 { get; private set; }
        /// <summary>スクリーンにはジェスチャーカーソルが表示されます。 (You should now see the Gesture Cursor on the screen.)</summary>
        public NarrationInformation View001_Message01200 { get; private set; }
        /// <summary>ジェスチャーカーソルの表示を消すには、手をジェスチャートラッキングエリアの外に出してください。 (To stop displaying the cursor, slide your hand out of the Gesture Tracking Area.)</summary>
        public NarrationInformation View001_Message01300 { get; private set; }
        /// <summary>手のトラッキングを終了しました。 (ZKOO has stopped tracking your hand.)</summary>
        public NarrationInformation View001_Message01400 { get; private set; }
        /// <summary>それでは、ZKOOに手を認識させる操作を、繰り返し練習してみましょう。 (Let's practice letting ZKOO detect your hand to bring up the Gesture Cursor.)</summary>
        public NarrationInformation View001_Message01500 { get; private set; }
        /// <summary>ZKOOに手を認識させて、手を降ろしてトラッキングを終了させることを、1分以内に5回行ってください。 (Try showing and hiding the Cursor 5 times.  You have 1 minute.)</summary>
        public NarrationInformation View001_Message01600 { get; private set; }
        /// <summary>ジェスチャーカーソルを動かす練習をしてみましょう。 (Let’s practice moving the Gesture Cursor.)</summary>
        public NarrationInformation View002_Message00100 { get; private set; }
        /// <summary>手を動かすとジェスチャーカーソルが移動します。 (When you move your hand, the Gesture Cursor moves along with it.)</summary>
        public NarrationInformation View002_Message00200 { get; private set; }
        /// <summary>1、2、3、4の順に、ジェスチャーカーソルを移動させてください。 (Move the Cursor to circles 1, 2, 3, and 4 in this order.)</summary>
        public NarrationInformation View002_Message00400 { get; private set; }
        /// <summary>1番の円まで、ジェスチャーカーソルを移動させてください。 (Move the Cursor to circle number 1.)</summary>
        public NarrationInformation View002_Message00501 { get; private set; }
        /// <summary>2番の円まで、ジェスチャーカーソルを移動させてください。 (Move the Cursor to circle number 2.)</summary>
        public NarrationInformation View002_Message00502 { get; private set; }
        /// <summary>3番の円まで、ジェスチャーカーソルを移動させてください。 (Move the Cursor to circle number 3.)</summary>
        public NarrationInformation View002_Message00503 { get; private set; }
        /// <summary>4番の円まで、ジェスチャーカーソルを移動させてください。 (Move the Cursor to circle number 4.)</summary>
        public NarrationInformation View002_Message00504 { get; private set; }
        /// <summary>慣れてくると、カメラビューを見なくても、ジェスチャーカーソルを見ながら操作ができるようになります。 (Once you get the hang of it, you should be able to easily control the Gesture Cursor without relying on the Camera View.)</summary>
        public NarrationInformation View002_Message00700 { get; private set; }
        /// <summary>タップジェスチャーを練習してみましょう。 (Let’s practice the tap gesture.)</summary>
        public NarrationInformation View003_Message00100 { get; private set; }
        /// <summary>1、2、3、4の円を、3回ずつタップしてください。 (Please tap on circles 1, 2, 3 and 4 in that order.  Repeat 3 times.)</summary>
        public NarrationInformation View003_Message01100 { get; private set; }
        /// <summary>1番の円まで、ジェスチャーカーソルを移動させてください。 (Move the Gesture Cursor to circle number 1.)</summary>
        public NarrationInformation View003_Message00200 { get; private set; }
        /// <summary>音声にタイミングを合わせて、指を曲げてください。 (Let's practice tapping.)</summary>
        public NarrationInformation View003_Message00400 { get; private set; }
        /// <summary>では、タップしてみましょう。1、2、3。 (Let's begin.  1, 2, 3.)</summary>
        public NarrationInformation View003_Message00700 { get; private set; }
        /// <summary>1番の円までジェスチャーカーソルを移動させて、3回タップしてください。 (Move the Cursor to circle number 1 and tap it 3 times.)</summary>
        public NarrationInformation View003_Message01201 { get; private set; }
        /// <summary>親指はのばしたまま、他の4本の指を軽く曲げてください。おいでおいでを行うように、指を曲げます。 (While keeping your thumb sticking out to the side, curl your 4 fingers toward your palm then quickly uncurl.)</summary>
        public NarrationInformation View003_Message00300 { get; private set; }
        /// <summary>1、2、3と数字を言う間に、指を曲げて伸ばす動作を素早く行います。 (A voice will count to 3.  Within that time, you will need to curl and quickly uncurl your fingers.)</summary>
        public NarrationInformation View003_Message00500 { get; private set; }
        /// <summary>もう一度、タップしてみましょう。1、2、3。 (Again.  1, 2, 3.)</summary>
        public NarrationInformation View003_Message00800 { get; private set; }
        /// <summary>指を長い間曲げたままにしておくと、ロングタップジェスチャーとなります。 (When you leave your fingers curled for a while, ZKOO detects it as a long press.)</summary>
        public NarrationInformation View003_Message00900 { get; private set; }
        /// <summary>指を曲げて伸ばす動作は、素早く行ってください。 (In order to tap, make sure you immediately uncurl your 4 fingers after the initial curl.)</summary>
        public NarrationInformation View003_Message01000 { get; private set; }
        /// <summary>2番の円を、3回タップしてください。 (Tap circle number 2 three times.)</summary>
        public NarrationInformation View003_Message01202 { get; private set; }
        /// <summary>3番の円を、3回タップしてください。 (Tap circle number 3 three times.)</summary>
        public NarrationInformation View003_Message01203 { get; private set; }
        /// <summary>4番の円を、3回タップしてください。 (Tap circle number 4 three times.)</summary>
        public NarrationInformation View003_Message01204 { get; private set; }
        /// <summary>ドラッグジェスチャーを練習してみましょう。 (Let’s practice the drag gesture.)</summary>
        public NarrationInformation View004_Message00100 { get; private set; }
        /// <summary>ドラッグ操作によって、1番の場所に置かれた小さな円を、2、3、4の場所に順番に動かしてください。 (Grab the small circle inside area 1 and drag it to areas 2, 3, 4 in that order.)</summary>
        public NarrationInformation View004_Message00900 { get; private set; }
        /// <summary>1番の場所に置かれた小さな円の上に、ジェスチャーカーソルを移動させてください。 (Move the Gesture Cursor to the small circle in the upper-right corner of the screen.)</summary>
        public NarrationInformation View004_Message00200 { get; private set; }
        /// <summary>親指はのばしたまま、他の4本の指を軽く曲げてください。 (Remember to keep your thumb sticking out while curling your other fingers.)</summary>
        public NarrationInformation View004_Message00400 { get; private set; }
        /// <summary>指を曲げたまま手を動かして、1番の大きな円の中にある小さな円を、2番まで移動させてください。 (While keeping your fingers curled, move your hand to drag the small circle inside of the bigger circle on the bottom right of the screen.)</summary>
        public NarrationInformation View004_Message00500 { get; private set; }
        /// <summary>小さな円が目標の場所まで移動したら、手を開きます。 (After moving the circle to the target area, please open your fingers.)</summary>
        public NarrationInformation View004_Message00700 { get; private set; }
        /// <summary>ドラッグ操作ができました。 (You have now successfully dragged an object.)</summary>
        public NarrationInformation View004_Message00800 { get; private set; }
        /// <summary>小さな円の上まで、ジェスチャーカーソルを移動させてください。 (Move the Gesture Cursor on top of the small circle.)</summary>
        public NarrationInformation View004_Message01100 { get; private set; }
        /// <summary>小さな円をタッチして、つかみます。 (Curl your 4 fingers to grab the small circle.)</summary>
        public NarrationInformation View004_Message01200 { get; private set; }
        /// <summary>1番にある小さな円を、2番まで移動させてください。 (Grab the small circle inside area 1 and drag it to area 2.)</summary>
        public NarrationInformation View004_Message01000 { get; private set; }
        /// <summary>指を曲げたまま、手を動かして、2番まで移動させてください。 (Keep your fingers curled and drag it to area 2.)</summary>
        public NarrationInformation View004_Message02102 { get; private set; }
        /// <summary>指を曲げたまま、手を動かして、3番まで移動させてください。 (Keep your fingers curled and drag it to area 3.)</summary>
        public NarrationInformation View004_Message02103 { get; private set; }
        /// <summary>指を曲げたまま、手を動かして、4番まで移動させてください。 (Keep your fingers curled and drag it to area 4.)</summary>
        public NarrationInformation View004_Message02104 { get; private set; }
        /// <summary>そのまま、2番まで移動させてください。 (Continue to move it to area 2.)</summary>
        public NarrationInformation View004_Message02202 { get; private set; }
        /// <summary>そのまま、3番まで移動させてください。 (Continue to move it to area 3.)</summary>
        public NarrationInformation View004_Message02203 { get; private set; }
        /// <summary>そのまま、4番まで移動させてください。 (Continue to move it to area 4.)</summary>
        public NarrationInformation View004_Message02204 { get; private set; }
        /// <summary>手を大きく動かす必要はありません。 (There is no need to make large movements with your hand.)</summary>
        public NarrationInformation View004_Message00600 { get; private set; }
        /// <summary>ドラッグを行うには、指を曲げたまま手を動かします。もう一度練習してみましょう。 (When dragging, keep your fingers curled with your thumb sticking out to the side.)</summary>
        public NarrationInformation View004_Message01600 { get; private set; }
        /// <summary>順番にドロップしてください。 (Drop them in order.)</summary>
        public NarrationInformation View004_Message01700 { get; private set; }
        /// <summary>フリックジェスチャーを練習してみましょう。 (Let’s practice the flick gesture.)</summary>
        public NarrationInformation View005_Message00100 { get; private set; }
        /// <summary>フリックジェスチャーで、四角い枠の中にあるコンテンツをスクロールしましょう。 (Let's scroll through the items inside the rectangular outlines, using the flick gesture.)</summary>
        public NarrationInformation View005_Message00200 { get; private set; }
        /// <summary>枠の中央あたりに、ジェスチャーカーソルを移動させてください。 (Move the Gesture Cursor to the center of the window.)</summary>
        public NarrationInformation View005_Message00300 { get; private set; }
        /// <summary>まずタッチ状態にします。親指はのばしたまま、他の4本の指を軽く曲げてください。 (Keep your thumb sticking out and curl your 4 fingers slightly, just like the drag gesture.)</summary>
        public NarrationInformation View005_Message00400 { get; private set; }
        /// <summary>手を小さく動かしながら、すぐに指をのばすことで、フリック操作ができます。 (To flick, start moving your hand slightly in one direction and open it quickly.)</summary>
        public NarrationInformation View005_Message00500 { get; private set; }
        /// <summary>手をそれほど大きく動かす必要はありません。 (There is no need to make large movements with your hand.)</summary>
        public NarrationInformation View005_Message00600 { get; private set; }
        /// <summary>手を動かしながら、手を開くと、フリック操作ができます。 (Just keep moving your hand and open it quickly to flick.)</summary>
        public NarrationInformation View005_Message00700 { get; private set; }
        /// <summary>ドラッグと違うのは、手を開くタイミングです。 (The timing for opening your hand is a bit different from dragging.)</summary>
        public NarrationInformation View005_Message00800 { get; private set; }
        /// <summary>指を曲げて、小さく移動しながら、すぐに手を開きます。 (Curl your fingers and while moving your hand in one direction, quickly open your hand.)</summary>
        public NarrationInformation View005_Message00900 { get; private set; }
        /// <summary>跳ね上げるように手を開くと、きれいにフリックができます。 (You can achieve a smooth flick by springing open your hands.)</summary>
        public NarrationInformation View005_Message01000 { get; private set; }
        /// <summary>これで、基本的なジェスチャー操作ができるようになりました。 (You have now learned all of the basic gestures.)</summary>
        public NarrationInformation View005_Message02700 { get; private set; }
        public NarrationInformationList()
        {
            View100_Message00100 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 1, SubIndex = 0, TextKey = "View100_Message00100_TextKey" };
            View100_Message00200 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 2, SubIndex = 0, TextKey = "View100_Message00200_TextKey" };
            View100_Message00300 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 3, SubIndex = 0, TextKey = "View100_Message00300_TextKey" };
            View100_Message00400 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 4, SubIndex = 0, TextKey = "View100_Message00400_TextKey" };
            View100_Message00500 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 5, SubIndex = 0, TextKey = "View100_Message00500_TextKey" };
            View100_Message00600 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 6, SubIndex = 0, TextKey = "View100_Message00600_TextKey" };
            View100_Message00700 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 7, SubIndex = 0, TextKey = "View100_Message00700_TextKey" };
            View100_Message00800 = new NarrationInformation() { ViewIndex = 100, MessageIndex = 8, SubIndex = 0, TextKey = "View100_Message00800_TextKey" };
            View101_Message00000 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 0, SubIndex = 0, TextKey = "View101_Message00000_TextKey" };
            View101_Message00100 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 1, SubIndex = 0, TextKey = "View101_Message00100_TextKey" };
            View101_Message00215 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 2, SubIndex = 15, TextKey = "View101_Message00215_TextKey" };
            View101_Message00225 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 2, SubIndex = 25, TextKey = "View101_Message00225_TextKey" };
            View101_Message00600 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 6, SubIndex = 0, TextKey = "View101_Message00600_TextKey" };
            View101_Message00201 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 2, SubIndex = 1, TextKey = "View101_Message00201_TextKey" };
            View101_Message00202 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 2, SubIndex = 2, TextKey = "View101_Message00202_TextKey" };
            View101_Message00203 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 2, SubIndex = 3, TextKey = "View101_Message00203_TextKey" };
            View101_Message00204 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 2, SubIndex = 4, TextKey = "View101_Message00204_TextKey" };
            View101_Message00300 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 3, SubIndex = 0, TextKey = "View101_Message00300_TextKey" };
            View101_Message00500 = new NarrationInformation() { ViewIndex = 101, MessageIndex = 5, SubIndex = 0, TextKey = "View101_Message00500_TextKey" };
            View001_Message00100 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 1, SubIndex = 0, TextKey = "View001_Message00100_TextKey" };
            View001_Message00200 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 2, SubIndex = 0, TextKey = "View001_Message00200_TextKey" };
            View001_Message00300 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 3, SubIndex = 0, TextKey = "View001_Message00300_TextKey" };
            View001_Message00400 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 4, SubIndex = 0, TextKey = "View001_Message00400_TextKey" };
            View001_Message00500 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 5, SubIndex = 0, TextKey = "View001_Message00500_TextKey" };
            View001_Message00600 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 6, SubIndex = 0, TextKey = "View001_Message00600_TextKey" };
            View001_Message00900 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 9, SubIndex = 0, TextKey = "View001_Message00900_TextKey" };
            View001_Message01000 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 10, SubIndex = 0, TextKey = "View001_Message01000_TextKey" };
            View001_Message01100 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 11, SubIndex = 0, TextKey = "View001_Message01100_TextKey" };
            View001_Message01200 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 12, SubIndex = 0, TextKey = "View001_Message01200_TextKey" };
            View001_Message01300 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 13, SubIndex = 0, TextKey = "View001_Message01300_TextKey" };
            View001_Message01400 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 14, SubIndex = 0, TextKey = "View001_Message01400_TextKey" };
            View001_Message01500 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 15, SubIndex = 0, TextKey = "View001_Message01500_TextKey" };
            View001_Message01600 = new NarrationInformation() { ViewIndex = 1, MessageIndex = 16, SubIndex = 0, TextKey = "View001_Message01600_TextKey" };
            View002_Message00100 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 1, SubIndex = 0, TextKey = "View002_Message00100_TextKey" };
            View002_Message00200 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 2, SubIndex = 0, TextKey = "View002_Message00200_TextKey" };
            View002_Message00400 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 4, SubIndex = 0, TextKey = "View002_Message00400_TextKey" };
            View002_Message00501 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 5, SubIndex = 1, TextKey = "View002_Message00501_TextKey" };
            View002_Message00502 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 5, SubIndex = 2, TextKey = "View002_Message00502_TextKey" };
            View002_Message00503 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 5, SubIndex = 3, TextKey = "View002_Message00503_TextKey" };
            View002_Message00504 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 5, SubIndex = 4, TextKey = "View002_Message00504_TextKey" };
            View002_Message00700 = new NarrationInformation() { ViewIndex = 2, MessageIndex = 7, SubIndex = 0, TextKey = "View002_Message00700_TextKey" };
            View003_Message00100 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 1, SubIndex = 0, TextKey = "View003_Message00100_TextKey" };
            View003_Message01100 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 11, SubIndex = 0, TextKey = "View003_Message01100_TextKey" };
            View003_Message00200 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 2, SubIndex = 0, TextKey = "View003_Message00200_TextKey" };
            View003_Message00400 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 4, SubIndex = 0, TextKey = "View003_Message00400_TextKey" };
            View003_Message00700 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 7, SubIndex = 0, TextKey = "View003_Message00700_TextKey" };
            View003_Message01201 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 12, SubIndex = 1, TextKey = "View003_Message01201_TextKey" };
            View003_Message00300 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 3, SubIndex = 0, TextKey = "View003_Message00300_TextKey" };
            View003_Message00500 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 5, SubIndex = 0, TextKey = "View003_Message00500_TextKey" };
            View003_Message00800 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 8, SubIndex = 0, TextKey = "View003_Message00800_TextKey" };
            View003_Message00900 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 9, SubIndex = 0, TextKey = "View003_Message00900_TextKey" };
            View003_Message01000 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 10, SubIndex = 0, TextKey = "View003_Message01000_TextKey" };
            View003_Message01202 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 12, SubIndex = 2, TextKey = "View003_Message01202_TextKey" };
            View003_Message01203 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 12, SubIndex = 3, TextKey = "View003_Message01203_TextKey" };
            View003_Message01204 = new NarrationInformation() { ViewIndex = 3, MessageIndex = 12, SubIndex = 4, TextKey = "View003_Message01204_TextKey" };
            View004_Message00100 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 1, SubIndex = 0, TextKey = "View004_Message00100_TextKey" };
            View004_Message00900 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 9, SubIndex = 0, TextKey = "View004_Message00900_TextKey" };
            View004_Message00200 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 2, SubIndex = 0, TextKey = "View004_Message00200_TextKey" };
            View004_Message00400 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 4, SubIndex = 0, TextKey = "View004_Message00400_TextKey" };
            View004_Message00500 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 5, SubIndex = 0, TextKey = "View004_Message00500_TextKey" };
            View004_Message00700 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 7, SubIndex = 0, TextKey = "View004_Message00700_TextKey" };
            View004_Message00800 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 8, SubIndex = 0, TextKey = "View004_Message00800_TextKey" };
            View004_Message01100 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 11, SubIndex = 0, TextKey = "View004_Message01100_TextKey" };
            View004_Message01200 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 12, SubIndex = 0, TextKey = "View004_Message01200_TextKey" };
            View004_Message01000 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 10, SubIndex = 0, TextKey = "View004_Message01000_TextKey" };
            View004_Message02102 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 21, SubIndex = 2, TextKey = "View004_Message02102_TextKey" };
            View004_Message02103 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 21, SubIndex = 3, TextKey = "View004_Message02103_TextKey" };
            View004_Message02104 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 21, SubIndex = 4, TextKey = "View004_Message02104_TextKey" };
            View004_Message02202 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 22, SubIndex = 2, TextKey = "View004_Message02202_TextKey" };
            View004_Message02203 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 22, SubIndex = 3, TextKey = "View004_Message02203_TextKey" };
            View004_Message02204 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 22, SubIndex = 4, TextKey = "View004_Message02204_TextKey" };
            View004_Message00600 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 6, SubIndex = 0, TextKey = "View004_Message00600_TextKey" };
            View004_Message01600 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 16, SubIndex = 0, TextKey = "View004_Message01600_TextKey" };
            View004_Message01700 = new NarrationInformation() { ViewIndex = 4, MessageIndex = 17, SubIndex = 0, TextKey = "View004_Message01700_TextKey" };
            View005_Message00100 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 1, SubIndex = 0, TextKey = "View005_Message00100_TextKey" };
            View005_Message00200 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 2, SubIndex = 0, TextKey = "View005_Message00200_TextKey" };
            View005_Message00300 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 3, SubIndex = 0, TextKey = "View005_Message00300_TextKey" };
            View005_Message00400 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 4, SubIndex = 0, TextKey = "View005_Message00400_TextKey" };
            View005_Message00500 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 5, SubIndex = 0, TextKey = "View005_Message00500_TextKey" };
            View005_Message00600 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 6, SubIndex = 0, TextKey = "View005_Message00600_TextKey" };
            View005_Message00700 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 7, SubIndex = 0, TextKey = "View005_Message00700_TextKey" };
            View005_Message00800 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 8, SubIndex = 0, TextKey = "View005_Message00800_TextKey" };
            View005_Message00900 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 9, SubIndex = 0, TextKey = "View005_Message00900_TextKey" };
            View005_Message01000 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 10, SubIndex = 0, TextKey = "View005_Message01000_TextKey" };
            View005_Message02700 = new NarrationInformation() { ViewIndex = 5, MessageIndex = 27, SubIndex = 0, TextKey = "View005_Message02700_TextKey" };
        }
    }
}
