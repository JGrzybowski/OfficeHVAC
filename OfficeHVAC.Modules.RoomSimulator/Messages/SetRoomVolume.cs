namespace OfficeHVAC.Modules.RoomSimulator.Messages
{
    public class SetRoomVolume
    {
        public double Volume { get; }
        public SetRoomVolume(double volume)
        {
            Volume = volume;
        }
        
    }
}