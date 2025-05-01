import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FeedbackService } from '../feedback.service';

@Component({
  selector: 'app-trainer-authentication',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './trainer-authentication.component.html',
  styleUrl: './trainer-authentication.component.css'
})
export class TrainerAuthenticationComponent {
  courseCode: string = '';
  secretCourseCode: string = '';
  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private feedbackService: FeedbackService,
    private router: Router
  ) {}

  async authenticate() {
    if (!this.courseCode || !this.secretCourseCode) {
      this.errorMessage = 'Please enter both course code and secret course code';
      return;
    }

    try {
      await this.feedbackService.getFeedbackOverview(this.courseCode, this.secretCourseCode);
      // If no error is thrown, authentication is successful
      this.router.navigate(['/trainer-overview'], { 
        queryParams: { 
          courseCode: this.courseCode, 
          secretCourseCode: this.secretCourseCode 
        }
      });
    } catch (error) {
      this.errorMessage = 'Invalid course code or secret course code';
    } 
  }
}
