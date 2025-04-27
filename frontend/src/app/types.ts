export type Course = {
  name: string;
  code: string;
};
export type CourseFeedback = {
  courseCode: string;
  courseName: string;
  feedbackStatus: string;
  numberOfParticipants: number;
  numberOfFeedbacksSubmitted: number;
  averageRatings: {
    helpful: number;
    satisfied: number;
    knowledgeable: number;
  };
  feedbackComments: {
    likedMostComments: string[];
    likedLeastComments: string[];
  };
};
