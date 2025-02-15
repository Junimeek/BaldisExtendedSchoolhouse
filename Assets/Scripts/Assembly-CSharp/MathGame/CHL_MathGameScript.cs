using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CHL_MathGameScript : MonoBehaviour
{
    private void Start()
    {   
        if (mathMusicScript == null) mathMusicScript = FindObjectOfType<MathMusicScript>();

        this.gc.ActivateLearningGame();
        this.problem = 0;

        this.NewProblem();
        if (this.gc.spoopMode) this.baldiFeedTransform.position = new Vector3(-1000f, -1000f, 0f);
    }

    private void Update()
    {
        if (!this.baldiAudio.isPlaying)
        {
            if (this.audioInQueue > 0 & (!this.gc.spoopMode))
            {
                this.PlayQueue();
            }
            this.baldiFeed.SetBool("talking", false);
        }
        else
        {
            this.baldiFeed.SetBool("talking", true);
        }
        if ((Input.GetKeyDown("return") || Input.GetKeyDown("enter")) & this.questionInProgress)
        {
            this.questionInProgress = false;
            this.CheckAnswer();
        }
        if (this.problem > 3)
        {
            this.endDelay -= 1f * Time.unscaledDeltaTime;
            if (this.endDelay <= 0f)
            {
                GC.Collect();
                this.ExitGame();
            }
        }
    }

    private void NewProblem()
    {
        if (!this.gc.spoopMode) this.mathMusicScript.PlaySong();

        this.playerAnswer.text = string.Empty;
        this.problem++;
        this.playerAnswer.ActivateInputField();

        if (this.problem <= 3)
        {
            this.QueueAudio(this.bal_problems[this.problem - 1]);

            int questionType;
            if (this.gc.isDifficultMath)
                questionType = Mathf.RoundToInt(UnityEngine.Random.Range(1f,4f));
            else questionType = Mathf.RoundToInt(UnityEngine.Random.Range(1f,2f));
            
            int digit1 = Mathf.RoundToInt(UnityEngine.Random.Range(0f,9f));
            int digit2 = Mathf.RoundToInt(UnityEngine.Random.Range(0f,9f));

            if (this.problem <= 2 || this.gc.notebooks <= 1)
            {
                switch(questionType)
                {
                    case 2:
                        NewSubtractionProblem(digit1, digit2);
                    break;
                    case 3:
                        NewMultiplicationProblem(digit1, digit2);
                    break;
                    case 4:
                        NewDivisionProblem(digit1, digit2);
                    break;
                    default:
                        NewAdditionProblem(digit1, digit2);
                    break;
                }
            }
            else
            {
                this.impossibleMode = true;
                this.NewImpossibleProblem(digit1, digit2);
            }

            this.questionInProgress = true;
        }
        else
        {
            this.endDelay = 5f;
            if (!this.gc.spoopMode)
            {
                if (randompraise == 0) this.endDelay = 3.4f;
                else if (randompraise == 1) this.endDelay = 1.5f;
                else if (randompraise == 2) this.endDelay = 2.4f;
                else if (randompraise == 3) this.endDelay = 3.5f;
                else if (randompraise == 4) this.endDelay = 3.7f;
                else if (randompraise == 5) this.endDelay = 4.6f;

                this.questionText.text = "WOW! YOU EXIST!";
            }
            else
            {
                if (gc.notebooks < 3) this.endDelay = 3f;
                else if (gc.notebooks > 2) this.endDelay = 1.5f;

                int num2 = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f));
                this.questionText.text = this.hintText[num2];
                this.questionText2.text = string.Empty;
                this.questionText3.text = string.Empty;
            }
        }
    }

    private void NewAdditionProblem(int digit1, int digit2)
    {
        this.solution = digit1 + digit2;
        this.questionText.text = string.Concat(new object[]
        {
            "SOLVE MATH Q",
            this.problem,
            ": \n \n",
            digit1,
            "+",
            digit2,
            "="
        });
        this.QueueAudio(this.bal_numbers[digit1]);
        this.QueueAudio(this.bal_plus);
        this.QueueAudio(this.bal_numbers[digit2]);
        this.QueueAudio(this.bal_equals);
    }

    private void NewSubtractionProblem(int digit1, int digit2)
    {
        this.solution = digit1 - digit2;
        this.questionText.text = string.Concat(new object[]
        {
            "SOLVE MATH Q",
            this.problem,
            ": \n \n",
            digit1,
            "-",
            digit2,
            "="
        });
        this.QueueAudio(this.bal_numbers[digit1]);
        this.QueueAudio(this.bal_minus);
        this.QueueAudio(this.bal_numbers[digit2]);
        this.QueueAudio(this.bal_equals);
    }

    private void NewMultiplicationProblem(int digit1, int digit2)
    {
        this.solution = digit1 * digit2;
        this.questionText.text = string.Concat(new object[]
        {
            "SOLVE MATH Q",
            this.problem,
            ": \n \n",
            digit1,
            "x",
            digit2,
            "="
        });
        this.QueueAudio(this.bal_numbers[digit1]);
        this.QueueAudio(this.bal_times);
        this.QueueAudio(this.bal_numbers[digit2]);
        this.QueueAudio(this.bal_equals);
    }

    private void NewDivisionProblem(int digit1, int digit2)
    {
        int index = digit1 + digit2;
        int newDigit1;
        int newDigit2;

        switch(index)
        {
            case 1: newDigit1 = 1; newDigit2 = 1; break; // 1÷1
            case 2: newDigit1 = 2; newDigit2 = 1; break; // 2÷1
            case 3: newDigit1 = 2; newDigit2 = 2; break; // 2÷2
            case 4: newDigit1 = 3; newDigit2 = 1; break; // 3÷1
            case 5: newDigit1 = 3; newDigit2 = 3; break; // 3÷3
            case 6: newDigit1 = 4; newDigit2 = 1; break; // 4÷1
            case 7: newDigit1 = 4; newDigit2 = 2; break; // 4÷2
            case 8: newDigit1 = 5; newDigit2 = 1; break; // 5÷1
            case 9: newDigit1 = 5; newDigit2 = 5; break; // 5÷5
            case 10: newDigit1 = 6; newDigit2 = 2; break; // 6÷2
            case 11: newDigit1 = 6; newDigit2 = 3; break; // 6÷3
            case 12: newDigit1 = 7; newDigit2 = 1; break; // 7÷1
            case 13: newDigit1 = 7; newDigit2 = 7; break; // 7÷7
            case 14: newDigit1 = 8; newDigit2 = 2; break; // 8÷2
            case 15: newDigit1 = 8; newDigit2 = 4; break; // 8÷4
            case 16: newDigit1 = 8; newDigit2 = 8; break; // 8÷8
            case 17: newDigit1 = 9; newDigit2 = 3; break; // 9÷3
            case 18: newDigit1 = 9; newDigit2 = 9; break; // 9÷9
            default: newDigit1 = 0; newDigit2 = 1; break; // 0÷1
        }
        this.solution = newDigit1 / newDigit2;
        this.questionText.text = string.Concat(new object[]
        {
            "SOLVE MATH Q",
            this.problem,
            ": \n \n",
            newDigit1,
            "÷",
            newDigit2,
            "="
        });
        this.QueueAudio(this.bal_numbers[newDigit1]);
        this.QueueAudio(this.bal_divided);
        this.QueueAudio(this.bal_numbers[newDigit2]);
        this.QueueAudio(this.bal_equals);
    }

    private void NewImpossibleProblem(int digit1, int digit2)
    {
        string sign1;
        string sign2;
        string sign3;
        float glitch1;
        float glitch2;
        float glitch3;

        switch(digit1)
        {
            case 1: sign1 = "-"; break;
            case 2: sign1 = "x"; break;
            case 3: sign1 = "*"; break;
            case 4: sign1 = "÷"; break;
            case 5: sign1 = "/"; break;
            case 6: sign1 = "%"; break;
            case 7: sign1 = "("; break;
            case 8: sign1 = ")"; break;
            case 9: sign1 = "!"; break;
            default: sign1 = "+"; break;
        }
        switch(digit2)
        {
            case 1: sign2 = "-"; break;
            case 2: sign2 = "x"; break;
            case 3: sign2 = "*"; break;
            case 4: sign2 = "÷"; break;
            case 5: sign2 = "/"; break;
            case 6: sign2 = "%"; break;
            case 7: sign2 = "("; break;
            case 8: sign2 = ")"; break;
            case 9: sign2 = "!"; break;
            default: sign2 = "+"; break;
        }
        switch(Mathf.Abs(digit1 - digit2))
        {
            case 1: sign3 = "+("; break;
            case 2: sign3 = "X"; break;
            case 3: sign3 = "/"; break;
            case 4: sign3 = ")+"; break;
            case 5: sign3 = "sqrt("; break;
            case 6: sign3 = "abs("; break;
            case 7: sign3 = "^"; break;
            case 8: sign3 = "y"; break;
            case 9: sign3 = "log("; break;
            default: sign3 = "e"; break;
        }

        glitch1 = UnityEngine.Random.Range(1f, 9999f);
        glitch2 = UnityEngine.Random.Range(1f, 9999f);
        glitch3 = UnityEngine.Random.Range(1f, 9999f);
        this.questionText.text = string.Concat(new object[]
        {
            "SOLVE MATH Q",
            this.problem,
            ": \n",
            glitch1, sign1, glitch2, sign2, glitch3,
            "="
        });

        glitch1 = UnityEngine.Random.Range(1f, 9999f);
        glitch2 = UnityEngine.Random.Range(1f, 9999f);
        glitch3 = UnityEngine.Random.Range(1f, 9999f);
        this.questionText2.text = string.Concat(new object[]
        {
            "SOLVE MATH Q",
            this.problem,
            ": \n",
            glitch1, sign2, glitch2, sign3, glitch3,
            "="
        });

        glitch1 = UnityEngine.Random.Range(1f, 9999f);
        glitch2 = UnityEngine.Random.Range(1f, 9999f);
        glitch3 = UnityEngine.Random.Range(1f, 9999f);
        this.questionText3.text = string.Concat(new object[]
        {
            "SOLVE MATH Q",
            this.problem,
            ": \n",
            glitch1, sign3, glitch2, sign1, glitch3,
            "="
        });

        this.QueueAudio(this.bal_screech);

        if (digit1 < 3) this.QueueAudio(this.bal_plus);
        else if (digit1 == 3 || digit1 == 4) this.QueueAudio(this.bal_minus);
        else if (digit1 == 5 || digit1 == 6) this.QueueAudio(this.bal_times);
        else this.QueueAudio(this.bal_divided);

        this.QueueAudio(this.bal_screech);

        if (digit2 < 3) this.QueueAudio(this.bal_plus);
        else if (digit2 == 3 || digit2 == 4) this.QueueAudio(this.bal_minus);
        else if (digit2 == 5 || digit2 == 6) this.QueueAudio(this.bal_times);
        else this.QueueAudio(this.bal_divided);

        this.QueueAudio(this.bal_screech);
        this.QueueAudio(this.bal_equals);
    }

    public void OKButton()
    {
        this.CheckAnswer();
    }

    public void CheckAnswer()
    {
        if (this.problem <= 3)
        {
            if (this.playerAnswer.text == this.solution.ToString() & !this.impossibleMode)
            {
                this.results[this.problem - 1].texture = this.correct;
                this.baldiAudio.Stop();
                this.ClearAudioQueue();
                int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 5f));
                this.randompraise = num;
                this.QueueAudio(this.bal_praises[num]);
                this.NewProblem();
            }
            else
            {
                this.problemsWrong++;
                this.results[this.problem - 1].texture = this.incorrect;

                this.baldiFeed.SetTrigger("angry");
                if (!gc.spoopMode) this.gc.ActivateSpoopMode();
                
                if (this.problem == 3 && this.impossibleMode) this.baldiScript.GetAngry(1f);

                this.ClearAudioQueue();
                this.baldiAudio.Stop();
                this.NewProblem();
            }
        }
    }

    private void QueueAudio(AudioClip sound)
    {
        this.audioQueue[this.audioInQueue] = sound;
        this.audioInQueue++;
    }

    private void PlayQueue()
    {
        this.baldiAudio.PlayOneShot(this.audioQueue[0]);
        this.UnqueueAudio();
    }

    private void UnqueueAudio()
    {
        for (int i = 1; i < this.audioInQueue; i++)
        {
            this.audioQueue[i - 1] = this.audioQueue[i];
        }
        this.audioInQueue--;
    }

    private void ClearAudioQueue()
    {
        this.audioInQueue = 0;
    }

    private void ExitGame()
    {
        if (this.problemsWrong == 0)
            this.gc.DeactivateLearningGame(base.gameObject, true);
        else this.gc.DeactivateLearningGame(base.gameObject, false);
    }

    public void ButtonPress(int value)
    {
        if (value >= 0 & value <= 9)
        {
            this.playerAnswer.text = this.playerAnswer.text + value;
        }
        else if (value == -1)
        {
            this.playerAnswer.text = this.playerAnswer.text + "-";
        }
        else
        {
            this.playerAnswer.text = string.Empty;
        }
    }

    private IEnumerator CheatText(string text)
    {
        for (; ; )
        {
            this.questionText.text = text;
            this.questionText2.text = string.Empty;
            this.questionText3.text = string.Empty;
            yield return new WaitForEndOfFrame();
        }
    }


    public ChallengeControllerScript gc;
    public BaldiScript baldiScript;
    public Vector3 playerPosition;
    public RawImage[] results = new RawImage[3];
    public Texture correct;
    public Texture incorrect;
    public TMP_InputField playerAnswer;
    public TMP_Text questionText;
    public TMP_Text questionText2;
    public TMP_Text questionText3;
    public Animator baldiFeed;
    public Transform baldiFeedTransform;
    public AudioClip bal_plus;
    public AudioClip bal_minus;
    public AudioClip bal_times;
    public AudioClip bal_divided;
    public AudioClip bal_equals;
    public AudioClip bal_screech;
    public AudioClip[] bal_numbers = new AudioClip[10];
    public AudioClip[] bal_praises = new AudioClip[5];
    public AudioClip[] bal_problems = new AudioClip[3];
    public Button firstButton;
    private float endDelay;
    public int problem;
    private int audioInQueue;
    private float solution;
    private string[] hintText = new string[]
    {
        "I GET ANGRIER FOR EVERY PROBLEM YOU GET WRONG",
        "I HEAR EVERY DOOR YOU OPEN",
        "(placeholder text; remember to fill in later)"
    };

    private bool questionInProgress;
    private bool impossibleMode;
    private int problemsWrong;
    [SerializeField] private AudioClip[] audioQueue = new AudioClip[20];
    public AudioSource baldiAudio;
    private int randompraise;
    [SerializeField] private MathMusicScript mathMusicScript;
}
