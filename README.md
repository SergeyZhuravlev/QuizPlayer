# QuizPlayer
Application for a showing quiz questions from quiz.json file.

Screens:
![This is an image](/assets/checkedquestion.png)
![This is an image](/assets/radioquestion.png)
![This is an image](/assets/quizresults.png)

QuizPlayer compile quiz.json to quiz.bin at a first application executing.
QuizPlayer show quiz at a second and others application executing.

File quiz.bin could be decompiled to json again by renaming quiz.bin to quiz.decompile and executing application QuizPlayer.

Source of quiz questions in quiz.json file:
![This is an image](/assets/quizjson.png)

Example of [quiz.json file](QuizPlayer/quiz.json)

quiz.json scheme:
* Array "Questions" stores all questions.
* Array "Answers" stores all answers of each question.
* Field "Text" stores text for answer or question. It could contain any symbols, but you should read about json format. E.g. \n symbol can be used for store NewLine symbol. This symbol suitable for multiline text.
* Field "RightAnswer"=true mark answer as right.
* Field "MustBeLastAnswer"=true force a answer to show below a not marked answers.
* Field "OnlyOneRightAnswerPerQuestion"=true in a question used for mark answer as contained only one right answer. This questions using radiobuttons for selecting answer. But questions with field "OnlyOneRightAnswerPerQuestion"=false could contain many right answer with checkboxes and a user must select all right answers for successfully complete this question.
* Field "OnlyOneRightAnswerPerQuestionByDefault" contain default value for a field "OnlyOneRightAnswerPerQuestion" when a question not contain a "OnlyOneRightAnswerPerQuestion" field. Field "OnlyOneRightAnswerPerQuestionByDefault" should be used for selecting for whole test question mode: a multianswer mode or a singleanswer mode. But you can override question mode for some of a question by field "OnlyOneRightAnswerPerQuestion" anyway.
* Field "QuizCaption" setup quiz name.
* Field "QuestionsPerQuiz" setup how many question will be shown before quiz will be ended.
* Field "MinimalAnsweredQuestionsPercentForQuizSuccess" setup success answered questions percent amount for successfully pass this test. Should be in range [0, 100] inclusively.
