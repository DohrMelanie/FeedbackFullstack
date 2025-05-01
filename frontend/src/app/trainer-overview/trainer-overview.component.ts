import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FeedbackService } from '../feedback.service';
import { CourseFeedback } from '../types';

@Component({
  selector: 'app-trainer-overview',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './trainer-overview.component.html',
  styleUrl: './trainer-overview.component.css'
})
export class TrainerOverviewComponent implements OnInit {
  courseCode: string = '';
  secretCourseCode: string = '';
  feedbackData: CourseFeedback | null = null;
  isLoading: boolean = true;
  error: string = '';
  
  showDeleteConfirmation: boolean = false;
  confirmCourseCode: string = '';
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private feedbackService: FeedbackService
  ) {}
  
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.courseCode = params['courseCode'];
      this.secretCourseCode = params['secretCourseCode'];
      
      if (!this.courseCode || !this.secretCourseCode) {
        this.router.navigate(['/login']);
        return;
      }
      
      this.loadFeedbackData();
    });
  }
  
  async loadFeedbackData(): Promise<void> {
    this.feedbackData = await this.feedbackService.getFeedbackOverview(this.courseCode, this.secretCourseCode);
    console.log(this.feedbackData!);
    console.log(this.feedbackData.courseCode);
    this.isLoading = false;
/*
    this.isLoading = true;
    this.error = '';
    
    try {
      this.error = 'Failed to load feedback data. Please check your credentials.';
      console.error('Error loading feedback data:', this.error);
    } finally {
      this.isLoading = false;
    }*/
  }
  
  async stopFeedback(): Promise<void> {
    if (!confirm('Are you sure you want to stop the feedback process? This will invalidate all feedback links.')) {
      return;
    }
    
    try {
      await this.feedbackService.stopCourse(this.courseCode, this.secretCourseCode);
      await this.loadFeedbackData();
    } catch (error) {
      this.error = 'Failed to stop feedback process.';
      console.error('Error stopping feedback:', error);
    }
  }
  
  openDeleteConfirmation(): void {
    this.showDeleteConfirmation = true;
    this.confirmCourseCode = '';
  }
  
  cancelDelete(): void {
    this.showDeleteConfirmation = false;
    this.confirmCourseCode = '';
  }
  
  async deleteCourse(): Promise<void> {
    if (this.confirmCourseCode !== this.courseCode) {
      this.error = 'Course code does not match.';
      return;
    }
    
    try {
      await this.feedbackService.deleteCourse(this.courseCode, this.secretCourseCode);
      this.router.navigate(['/login'], { queryParams: { deleted: 'true' } });
    } catch (error) {
      this.error = 'Failed to delete course.';
      console.error('Error deleting course:', error);
    }
  }
  
  hasFeedback(): boolean {
    return !!this.feedbackData?.numberOfFeedbacksSubmitted;
  }
  
  isFeedbackStopped(): boolean {
    return this.feedbackData?.feedbackStatus === 'Closed';
  }
}
