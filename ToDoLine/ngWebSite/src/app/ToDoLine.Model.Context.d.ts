/// <reference path="../../node_modules/@bit/jaydata/jaydata.d.ts" />

declare module ToDoLine.Dto {
	
	class ToDoItemStepDto extends $data.Entity {
				    
			Id : string;
			static Id : any;
				    
			Text : string;
			static Text : any;
				    
			ToDoItemId : string;
			static ToDoItemId : any;
				    
			IsCompleted : boolean;
			static IsCompleted : any;
			}
}


declare module ToDoLine.Dto {
	
	class ToDoGroupDto extends $data.Entity {
				    
			Id : string;
			static Id : any;
				    
			Title : string;
			static Title : any;
				    
			CreatedBy : string;
			static CreatedBy : any;
				    
			CreatedOn : Date;
			static CreatedOn : any;
				    
			ModifiedOn : Date;
			static ModifiedOn : any;
				    
			Theme : string;
			static Theme : any;
				    
			SortedBy : ToDoLine.Enum.SortBy;
			static SortedBy : any;
				    
			HideCompletedToDoItems : boolean;
			static HideCompletedToDoItems : any;
				    
			SharedByCount : number;
			static SharedByCount : any;
			}
}


declare module ToDoLine.Dto {
	
	class UserRegistrationDto extends $data.Entity {
				    
			UserName : string;
			static UserName : any;
				    
			Password : string;
			static Password : any;
			}
}


declare module ToDoLine.Dto {
	
	class UserDto extends $data.Entity {
				    
			Id : string;
			static Id : any;
				    
			UserName : string;
			static UserName : any;
			}
}


declare module ToDoLine.Dto {
	
	class ToDoItemDto extends $data.Entity {
				    
			Id : string;
			static Id : any;
				    
			Title : string;
			static Title : any;
				    
			IsImportant : boolean;
			static IsImportant : any;
				    
			IsCompleted : boolean;
			static IsCompleted : any;
				    
			Notes : string;
			static Notes : any;
				    
			DueDate : Date;
			static DueDate : any;
				    
			CreatedBy : string;
			static CreatedBy : any;
				    
			CompletedBy : string;
			static CompletedBy : any;
				    
			CreatedOn : Date;
			static CreatedOn : any;
				    
			ModifiedOn : Date;
			static ModifiedOn : any;
				    
			RemindOn : Date;
			static RemindOn : any;
				    
			ShowInMyDay : boolean;
			static ShowInMyDay : any;
				    
			ToDoGroupId : string;
			static ToDoGroupId : any;
				    
			ToDoItemStepsCount : number;
			static ToDoItemStepsCount : any;
				    
			ToDoItemStepsCompletedCount : number;
			static ToDoItemStepsCompletedCount : any;
			}
}



declare module ToDoLine.Enum {
	
	enum SortBy {
				    /**Value: 0*/
			CreatedDate,
				    /**Value: 1*/
			Importance,
			}
}



    
	interface ToDoItemsEntitySet extends $data.EntitySet<ToDoLine.Dto.ToDoItemDto>{
				    
		    getMyToDoItems():  $data.Queryable<ToDoLine.Dto.ToDoItemDto> ;
			}
    
	interface UsersEntitySet extends $data.EntitySet<ToDoLine.Dto.UserDto>{
				    
		    getAllUsers():  $data.Queryable<ToDoLine.Dto.UserDto> ;
			}
    
	interface UserRegistrationEntitySet extends $data.EntitySet<ToDoLine.Dto.UserRegistrationDto>{
				    
		    register(userRegistration : ToDoLine.Dto.UserRegistrationDto):  Promise<void> ;
			}
    
	interface ToDoGroupsEntitySet extends $data.EntitySet<ToDoLine.Dto.ToDoGroupDto>{
				    
		    getMyToDoGroups():  $data.Queryable<ToDoLine.Dto.ToDoGroupDto> ;
				    
		    createToDoGroup(title : string):  Promise<ToDoLine.Dto.ToDoGroupDto> ;
				    
		    shareToDoGroupWithAnotherUser(toDoGroupId : string,anotherUserId : string):  Promise<ToDoLine.Dto.ToDoGroupDto> ;
				    
		    kickUserFromToDoGroup(userId : string,toDoGroupId : string):  Promise<void> ;
			}
    
	interface ToDoItemStepsEntitySet extends $data.EntitySet<ToDoLine.Dto.ToDoItemStepDto>{
				    
		    getToDoItemSteps(toDoItemId : string):  $data.Queryable<ToDoLine.Dto.ToDoItemStepDto> ;
			}

declare class ToDoLineContext extends $data.EntityContext {

		    
		toDoItems: ToDoItemsEntitySet;
		    
		users: UsersEntitySet;
		    
		userRegistration: UserRegistrationEntitySet;
		    
		toDoGroups: ToDoGroupsEntitySet;
		    
		toDoItemSteps: ToDoItemStepsEntitySet;
	
}


