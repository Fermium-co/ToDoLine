

		 var   ToDoLine = ToDoLine || {};
		 ToDoLine.Dto = ToDoLine.Dto || {};

ToDoLine.Dto.ToDoItemStepDto = $data.Entity.extend("ToDoLine.Dto.ToDoItemStepDto", {
	 
		Id: {
			"type": "Edm.Guid" , nullable: false
															, "key": true
			, "required" : true
			, "computed": true
										},
	 
		Text: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		ToDoItemId: {
			"type": "Edm.Guid" , nullable: false
																			},
	 
		IsCompleted: {
			"type": "Edm.Boolean" , nullable: false
																			},
		});


		 var   ToDoLine = ToDoLine || {};
		 ToDoLine.Dto = ToDoLine.Dto || {};

ToDoLine.Dto.ToDoGroupDto = $data.Entity.extend("ToDoLine.Dto.ToDoGroupDto", {
	 
		Id: {
			"type": "Edm.Guid" , nullable: false
															, "key": true
			, "required" : true
			, "computed": true
										},
	 
		Title: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		CreatedBy: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		CreatedOn: {
			"type": "Edm.DateTimeOffset" , nullable: false
																			},
	 
		ModifiedOn: {
			"type": "Edm.DateTimeOffset" , nullable: false
																			},
	 
		Theme: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		SortedBy: {
			"type": "ToDoLine.Enum.SortBy" , nullable: false
																			},
	 
		HideCompletedToDoItems: {
			"type": "Edm.Boolean" , nullable: false
																			},
	 
		SharedByCount: {
			"type": "Edm.Int32" , nullable: false
																			},
		});


		 var   ToDoLine = ToDoLine || {};
		 ToDoLine.Dto = ToDoLine.Dto || {};

ToDoLine.Dto.UserRegistrationDto = $data.Entity.extend("ToDoLine.Dto.UserRegistrationDto", {
	 
		UserName: {
			"type": "Edm.String" , nullable: true
															, "key": true
			, "required" : false
			, "computed": true
										},
	 
		Password: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
		});


		 var   ToDoLine = ToDoLine || {};
		 ToDoLine.Dto = ToDoLine.Dto || {};

ToDoLine.Dto.UserDto = $data.Entity.extend("ToDoLine.Dto.UserDto", {
	 
		Id: {
			"type": "Edm.Guid" , nullable: false
															, "key": true
			, "required" : true
			, "computed": true
										},
	 
		UserName: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
		});


		 var   ToDoLine = ToDoLine || {};
		 ToDoLine.Dto = ToDoLine.Dto || {};

ToDoLine.Dto.ToDoItemDto = $data.Entity.extend("ToDoLine.Dto.ToDoItemDto", {
	 
		Id: {
			"type": "Edm.Guid" , nullable: false
															, "key": true
			, "required" : true
			, "computed": true
										},
	 
		Title: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		IsImportant: {
			"type": "Edm.Boolean" , nullable: false
																			},
	 
		IsCompleted: {
			"type": "Edm.Boolean" , nullable: false
																			},
	 
		Notes: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		DueDate: {
			"type": "Edm.DateTimeOffset" , nullable: true
			  , defaultValue: null
																			},
	 
		CreatedBy: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		CompletedBy: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		CreatedOn: {
			"type": "Edm.DateTimeOffset" , nullable: false
																			},
	 
		ModifiedOn: {
			"type": "Edm.DateTimeOffset" , nullable: false
																			},
	 
		RemindOn: {
			"type": "Edm.DateTimeOffset" , nullable: true
			  , defaultValue: null
																			},
	 
		ShowInMyDay: {
			"type": "Edm.Boolean" , nullable: false
																			},
	 
		ToDoGroupId: {
			"type": "Edm.Guid" , nullable: true
			  , defaultValue: null
																			},
	 
		ToDoItemStepsCount: {
			"type": "Edm.Int32" , nullable: false
																			},
	 
		ToDoItemStepsCompletedCount: {
			"type": "Edm.Int32" , nullable: false
																			},
		});



		 var   ToDoLine = ToDoLine || {};
		 ToDoLine.Enum = ToDoLine.Enum || {};

ToDoLine.Enum.SortBy = $data.createEnum("ToDoLine.Enum.SortBy", null, $data.String, [
	 
		{ name : 'CreatedDate' , value : "ToDoLine.Enum.SortBy'CreatedDate'" , index : 0 },
	 
		{ name : 'Importance' , value : "ToDoLine.Enum.SortBy'Importance'" , index : 1 },
		]);


ToDoLineContext = $data.EntityContext.extend("ToDoLineContext", {
			toDoItems : {
			"type": "$data.EntitySet",
			"elementType": "ToDoLine.Dto.ToDoItemDto",
							"actions": {
													"getMyToDoItems": {
								"type": "$data.ServiceFunction",
								"namespace": "Default",
								"returnType":  "$data.Queryable" ,
								 "elementType": "ToDoLine.Dto.ToDoItemDto", 
																	"params": [
																			]
						},
										}
					},
			users : {
			"type": "$data.EntitySet",
			"elementType": "ToDoLine.Dto.UserDto",
							"actions": {
													"getAllUsers": {
								"type": "$data.ServiceFunction",
								"namespace": "Default",
								"returnType":  "$data.Queryable" ,
								 "elementType": "ToDoLine.Dto.UserDto", 
																	"params": [
																			]
						},
										}
					},
			userRegistration : {
			"type": "$data.EntitySet",
			"elementType": "ToDoLine.Dto.UserRegistrationDto",
							"actions": {
													"register": {
								"type": "$data.ServiceAction",
								"namespace": "Default",
								"returnType":  null ,
																	"params": [
																					{
												"name": "userRegistration",
												"type": "ToDoLine.Dto.UserRegistrationDto",
																							},									
																			]
						},
										}
					},
			toDoGroups : {
			"type": "$data.EntitySet",
			"elementType": "ToDoLine.Dto.ToDoGroupDto",
							"actions": {
													"getMyToDoGroups": {
								"type": "$data.ServiceFunction",
								"namespace": "Default",
								"returnType":  "$data.Queryable" ,
								 "elementType": "ToDoLine.Dto.ToDoGroupDto", 
																	"params": [
																			]
						},
													"createToDoGroup": {
								"type": "$data.ServiceAction",
								"namespace": "Default",
								"returnType":  "ToDoLine.Dto.ToDoGroupDto" ,
																	"params": [
																					{
												"name": "title",
												"type": "Edm.String",
																							},									
																			]
						},
													"shareToDoGroupWithAnotherUser": {
								"type": "$data.ServiceAction",
								"namespace": "Default",
								"returnType":  "ToDoLine.Dto.ToDoGroupDto" ,
																	"params": [
																					{
												"name": "toDoGroupId",
												"type": "Edm.Guid",
																							},									
																					{
												"name": "anotherUserId",
												"type": "Edm.Guid",
																							},									
																			]
						},
													"kickUserFromToDoGroup": {
								"type": "$data.ServiceAction",
								"namespace": "Default",
								"returnType":  null ,
																	"params": [
																					{
												"name": "userId",
												"type": "Edm.Guid",
																							},									
																					{
												"name": "toDoGroupId",
												"type": "Edm.Guid",
																							},									
																			]
						},
										}
					},
			toDoItemSteps : {
			"type": "$data.EntitySet",
			"elementType": "ToDoLine.Dto.ToDoItemStepDto",
							"actions": {
													"getToDoItemSteps": {
								"type": "$data.ServiceFunction",
								"namespace": "Default",
								"returnType":  "$data.Queryable" ,
								 "elementType": "ToDoLine.Dto.ToDoItemStepDto", 
																	"params": [
																					{
												"name": "toDoItemId",
												"type": "Edm.Guid",
																							},									
																			]
						},
										}
					},
	});


