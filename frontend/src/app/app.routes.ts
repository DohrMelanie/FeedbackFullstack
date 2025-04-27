import { Routes } from '@angular/router';
import { FeedbackComponent } from './feedback/feedback.component';
import { TrainerAuthenticationComponent } from './trainer-authentication/trainer-authentication.component';
import { TrainerOverviewComponent } from './trainer-overview/trainer-overview.component';

export const routes: Routes = [
    { path: 'feedback/:code', component: FeedbackComponent },
    { path: 'login', component: TrainerAuthenticationComponent},
    { path: 'trainer-overview', component: TrainerOverviewComponent},
    { path: '', redirectTo: 'login', pathMatch: 'full' },
];
