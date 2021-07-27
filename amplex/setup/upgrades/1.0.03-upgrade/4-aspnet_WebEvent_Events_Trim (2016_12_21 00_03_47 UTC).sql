if exists ( select [id] from sysobjects where [name] = 'aspnet_WebEvent_Events_Trim' )
	drop procedure aspnet_WebEvent_Events_Trim
go

create procedure aspnet_WebEvent_Events_Trim
(
	@MaxEvents int,
	@TrimEvents int
)
as begin
	if( IsNull(@MaxEvents, 0) = 0 )
		delete aspnet_WebEvent_Events
	else
	begin
		declare @CurrentCount int
		select @CurrentCount = count(EventId)
		from aspnet_webevent_events
		
		if @CurrentCount > @MaxEvents 
		begin
		
			declare @deleteIds table
			(
				idx int primary key identity,
				EventId char(32) 
			)
			
			insert @deleteIds 
			select EventId 
			from aspnet_webevent_events
			
			delete @deleteIds 
			where idx <= @TrimEvents
			
			delete aspnet_webevent_events
			from aspnet_webevent_events e, @deleteIds d
			where e.eventid = d.eventid
		end
	end
end
go



