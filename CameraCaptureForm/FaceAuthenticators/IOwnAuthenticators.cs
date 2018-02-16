using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceAuthenticators
{
    public interface IOwnAuthenticators
    {
        void TrainAuthenticator();
        string PredictImage(Image<Gray, byte> inputImage);
    }
}
